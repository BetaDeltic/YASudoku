using CommunityToolkit.Mvvm.Input;
using YASudoku.Models;
using YASudoku.Models.PuzzleGenerator;
using YASudoku.Services.JournalingServices;
using YASudoku.Services.SettingsService;
using YASudoku.ViewModels.GameViewModel.Commands;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel;

public partial class GameVM : VMsBase, IDisposable
{
    public readonly int gridSize = 9;

    private readonly IPuzzleGenerator generator;
    private readonly IPlayerJournalingService journal;
    public readonly IServiceProvider serviceProvider;

    public VisualStatesHandler? VisualState { get; private set; }

    private SwitchPenAndPencilCmd? switchPenAndPencilCmd;
    private PressNumberCmd? pressNumberCmd;
    private SelectEraserCmd? selectEraserCmd;
    private SelectCellCmd? selectCellCmd;
    private NewGameCmd? newGameCmd;
    private RestartGameCmd? restartGameCmd;
    private PauseGameCmd? pauseGameCmd;
    private SettingsCmd? settingsCmd;
    private UndoCmd? undoCmd;

    public GameVM( IPuzzleGenerator puzzleGenerator, ISettingsService settingsService, IServiceProvider serviceProvider,
        IPlayerJournalingService journalingService )
        : base( settingsService )
    {
        this.serviceProvider = serviceProvider;
        journal = journalingService;
        generator = puzzleGenerator;
    }

    public void Dispose()
    {
        VisualState?.Dispose();
        GC.SuppressFinalize( this );
    }

    public void PrepareGameView( bool generateNew )
    {
        GameDataContainer gameData = GetPuzzleData( generateNew );
        GameGridVisualDataCollection visualData = new( gameData.AllCells, gridSize, serviceProvider );

        VisualState = new( gridSize, visualData, serviceProvider );

        journal.SetVisualState( VisualState );

        switchPenAndPencilCmd = new( VisualState.PencilVS );
        pressNumberCmd = new( VisualState );
        selectCellCmd = new( VisualState );
        selectEraserCmd = new( VisualState );
        newGameCmd = new( VisualState, generator, journal );
        restartGameCmd = new( VisualState );
        pauseGameCmd = new( VisualState );
        settingsCmd = new( VisualState.SettingsVS );
        undoCmd = new( journal );

        VisualState.UpdateAllButtonRemainingCounts();
    }

    private GameDataContainer GetPuzzleData( bool generateNew )
    {
        GameDataContainer gameData;
        if ( generateNew ) {
            gameData = generator.GenerateNewPuzzle();
        } else {
            PuzzleLoader loader = new();
            gameData = generator.GenerateNewPuzzle();
        }
        return gameData;
    }

    [RelayCommand]
    public void StartNewGame() => newGameCmd?.NewGame();

    [RelayCommand]
    public void RestartGame() => restartGameCmd?.RestartGame();

    [RelayCommand]
    public void PauseGame() => pauseGameCmd?.TogglePausedState();

    [RelayCommand]
    public void SwitchPenAndPencil() => switchPenAndPencilCmd?.SwitchPenAndPencil();

    [RelayCommand]
    public void PressNumber( int pressedNumber ) => pressNumberCmd?.PressNumber( pressedNumber );

    [RelayCommand]
    public void SelectEraser() => selectEraserCmd?.SelectEraser();

    [RelayCommand]
    public void SelectCell( int selectedCellIndex ) => selectCellCmd?.SelectCell( selectedCellIndex );

    [RelayCommand]
    public void OpenSettings() => settingsCmd?.OpenSettings();

    [RelayCommand]
    public void UndoLastAction() => undoCmd?.UndoLastActionInJournal();

    public void OnPageAppearing() => VisualState?.StartGame();
}
