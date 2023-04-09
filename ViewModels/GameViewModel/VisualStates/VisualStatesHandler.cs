using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using YASudoku.Common;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public enum VisualStates { Starting, Running, Finished };

public partial class VisualStatesHandler : ObservableObject, IDisposable
{
    public event Action? Victory;
    public event Action? NewGameData;

    public readonly GameGridVisualState GameGridVS;
    public readonly CommonButtonVisualState PencilVS;
    public readonly CommonButtonVisualState EraserVS;
    public readonly CommonButtonVisualState PauseVS;
    public readonly NumPadVisualState NumPadVS;
    public readonly CommonButtonVisualState OtherButtonsVS;

    public GameGridVisualDataCollection GameData
    {
        get => _gamedata!;
        set {
            _gamedata = value;
            NewGameData?.Invoke();
        }
    }

    private GameGridVisualDataCollection? _gamedata;

    public GameGridCellVisualData? SelectedCell => GameGridVS.SelectedCell;
    public NumPadButton? SelectedNumber => NumPadVS.SelectedButton;

    public List<NumPadButton> NumPadButtons => NumPadVS.NumPadButtons;

    public bool IsPencilActive => PencilVS.IsActive;
    public bool IsEraserActive => EraserVS.IsActive;
    public bool IsPaused => PauseVS.IsActive;

    public VisualStates CurrentVisualState = VisualStates.Starting;

    [ObservableProperty]
    private string _timerText = "00:00";

    [ObservableProperty]
    private bool _areSettingsVisible;

    private readonly int gridSize;

    private System.Timers.Timer? timer;

    private int totalElapsedTime;

    public VisualStatesHandler( int gridSize, GameGridVisualDataCollection gameData, IServiceProvider serviceProvider )
    {
        this.gridSize = gridSize;
        GameData = gameData;
        ISettingsService settingsService = serviceProvider.GetService<ISettingsService>()!;

        PencilVS = new( settingsService );
        EraserVS = new( settingsService );
        PauseVS = new( settingsService );
        OtherButtonsVS = new( settingsService );
        NumPadVS = new( gridSize );
        GameGridVS = new( settingsService, gameData );

        PencilVS.PropertyChanged += UnderlyingVS_PropertyChanged;
        EraserVS.PropertyChanged += UnderlyingVS_PropertyChanged;
        PauseVS.PropertyChanged += UnderlyingVS_PropertyChanged;
        OtherButtonsVS.PropertyChanged += UnderlyingVS_PropertyChanged;
        NumPadVS.NewNumberSelected += NumPadVS_NewNumberSelected;
        GameGridVS.NumberCountChanged += GameGridVS_NumberCountChanged;

        NewGameData += VisualStatesHandler_NewGameData;
    }

    public void Dispose()
    {
        timer?.Dispose();
        GC.SuppressFinalize( this );
    }

    private void VisualStatesHandler_NewGameData()
        => GameGridVS.ChangeCellData( GameData );

    public void StartGame()
    {
        StartTimer();
        CurrentVisualState = VisualStates.Running;
    }

    public void StartTimer( bool startFromZero = true )
    {
        if ( startFromZero ) {
            totalElapsedTime = 0;
            if ( timer != null ) {
                timer.Stop();
                timer.Elapsed -= Timer_Elapsed;
                timer.Dispose();
            }
        }

        timer = new( TimeSpan.FromSeconds( 1 ) ) {
            AutoReset = true,
            Enabled = true,
        };
        timer.Elapsed += Timer_Elapsed;
    }

    private void Timer_Elapsed( object? sender, System.Timers.ElapsedEventArgs e )
    {
        totalElapsedTime++;
        TimeSpan elapsedTimeSpan = TimeSpan.FromSeconds( totalElapsedTime );
        TimerText = elapsedTimeSpan.ToString( @"mm\:ss" );
    }

    public void UnpauseTimer() => timer?.Start();

    public void StopTimer() => timer?.Stop();

    public void UpdateAllButtonRemainingCounts()
        => Enumerable.Range( 1, gridSize )
        .ForEach( UpdateButtonRemainingCount );

    public void UpdateButtonRemainingCount( int buttonNumber )
    {
        if ( buttonNumber <= 0 ) {
            return;
        }

        int numberCount = GameGridVS.GetCellsWithSameNumber( buttonNumber ).Count();
        int remainingCount = gridSize - numberCount;

        NumPadVS.UpdateButtonRemainingCount( buttonNumber, remainingCount );
    }

    private void GameGridVS_NumberCountChanged( int changedNumber, GameGridCellVisualData changedCell )
    {
        UpdateButtonRemainingCount( changedNumber );
        CheckVictoryConditionsAndNotifyOnVictory( changedCell );
    }

    private void CheckVictoryConditionsAndNotifyOnVictory( GameGridCellVisualData recentlyChangedCell )
    {
        if ( !recentlyChangedCell.HasCorrectValue || ThereAreEmptyCells() || ThereAreIncorrectCells() ) return;

        CurrentVisualState = VisualStates.Finished;
        StopTimer();
        Victory?.Invoke();
    }

    private bool ThereAreEmptyCells()
        => GameData.Where( cell => !cell.HasUserFacingValue ).Any();

    private bool ThereAreIncorrectCells()
        => GameData.Where( cell => cell.HasUserFacingValue && !cell.HasCorrectValue ).Any();

    private void NumPadVS_NewNumberSelected( int newNumberSelected )
        => GameGridVS.HighlightCellsWithSameNumber( newNumberSelected );

    private void UnderlyingVS_PropertyChanged( object? sender, PropertyChangedEventArgs e )
    {
        if ( e.PropertyName != null )
            OnPropertyChanged( e.PropertyName );
    }
}
