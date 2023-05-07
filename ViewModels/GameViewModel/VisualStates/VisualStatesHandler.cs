using System.Reactive;
using System.Reactive.Subjects;
using YASudoku.Common;
using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public enum GameStates { Starting, Running, Finished };

public class VisualStatesHandler : IDisposable
{
    public event Action? Victory;
    public event Action? NewGameData;

    public readonly Subject<Unit> SignalWhenWipingGameBoard = new();
    public readonly BehaviorSubject<bool> WipingGameBoardCompleted = new( false );
    public readonly Subject<Unit> SignalWhenStartingNewGame = new();
    public readonly BehaviorSubject<bool> StartingNewGameCompleted = new( false );

    public readonly GameGridVisualState GameGridVS;
    public readonly CommonButtonVisualState PencilVS;
    public readonly CommonButtonVisualState EraserVS;
    public readonly CommonButtonVisualState PauseVS;
    public readonly NumPadVisualState NumPadVS;
    public readonly CommonButtonVisualState OtherButtonsVS;
    public readonly TimerVisualState TimerVS = new();
    public readonly SettingsVisualState SettingsVS = new();

    private GameGridVisualDataCollection? _gamedata;

    public GameGridVisualDataCollection GameData
    {
        get => _gamedata!;
        set {
            _gamedata = value;
            NewGameData?.Invoke();
        }
    }

    public GameGridCellVisualData? SelectedCell => GameGridVS.SelectedCell;
    public NumPadButton? SelectedNumber => NumPadVS.SelectedButton;

    public List<NumPadButton> NumPadButtons => NumPadVS.NumPadButtons;

    public bool IsPencilActive => PencilVS.IsActive;
    public bool IsEraserActive => EraserVS.IsActive;
    public bool IsPaused => PauseVS.IsActive;

    public GameStates CurrentGameState { get; private set; } = GameStates.Starting;

    private readonly int gridSize;

    public VisualStatesHandler( int gridSize, GameGridVisualDataCollection gameData, IServiceProvider serviceProvider )
    {
        this.gridSize = gridSize;
        GameData = gameData;
        ISettingsService settingsService = serviceProvider.GetService<ISettingsService>()!;
        IResourcesService resourcesService = serviceProvider.GetService<IResourcesService>()!;

        PencilVS = new( settingsService, resourcesService );
        EraserVS = new( settingsService, resourcesService );
        PauseVS = new( settingsService, resourcesService );
        OtherButtonsVS = new( settingsService, resourcesService );
        NumPadVS = new( gridSize, settingsService, resourcesService );
        GameGridVS = new( settingsService, gameData );

        NumPadVS.NewNumberSelected += NumPadVS_NewNumberSelected;
        GameGridVS.NumberCountChanged += GameGridVS_NumberCountChanged;

        NewGameData += VisualStatesHandler_NewGameData;
    }

    public void Dispose()
    {
        TimerVS.Dispose();
        GC.SuppressFinalize( this );
    }

    private void NumPadVS_NewNumberSelected( int newNumberSelected )
        => GameGridVS.HighlightCellsWithSameNumber( newNumberSelected );

    private void GameGridVS_NumberCountChanged( int changedNumber, GameGridCellVisualData changedCell )
    {
        UpdateButtonRemainingCount( changedNumber );
        CheckVictoryConditionsAndNotifyOnVictory( changedCell );
    }

    private void CheckVictoryConditionsAndNotifyOnVictory( GameGridCellVisualData recentlyChangedCell )
    {
        if ( !recentlyChangedCell.HasCorrectValue || ThereAreEmptyCells() || ThereAreIncorrectCells() ) return;

        CurrentGameState = GameStates.Finished;
        TimerVS.StopAndDisposeTimer();
        Victory?.Invoke();
    }

    private bool ThereAreEmptyCells() => GameData.Any( cell => !cell.HasUserFacingValue );

    private bool ThereAreIncorrectCells() => GameData.Any( cell => !cell.HasCorrectValue );

    private void VisualStatesHandler_NewGameData() => GameGridVS.ChangeCellData( GameData );

    public void ResetVisualStatesToDefault()
    {
        if ( IsPaused ) {
            GameGridVS.ShowAllCellValues();
            PauseVS.DeactivateButton();
        }

        NumPadVS.DeselectCurrentNumber();
        GameGridVS.DeselectCell();
        GameGridVS.UnhighlightCellsWithSameNumber();

        if ( IsPencilActive ) PencilVS.DeactivateButton();

        if ( IsEraserActive ) EraserVS.DeactivateButton();
    }

    public void StartGame()
    {
        TimerVS.StartTimer();
        CurrentGameState = GameStates.Running;
    }

    public void UpdateAllButtonRemainingCounts() => Enumerable.Range( 1, gridSize ).ForEach( UpdateButtonRemainingCount );

    public void UpdateButtonRemainingCount( int buttonNumber )
    {
        if ( buttonNumber <= 0 ) {
            return;
        }

        int numberCount = GameGridVS.GetCellsWithSameNumber( buttonNumber ).Count();
        int remainingCount = gridSize - numberCount;

        NumPadVS.UpdateButtonRemainingCount( buttonNumber, remainingCount );
    }
}
