using CommunityToolkit.Mvvm.Input;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using YASudoku.Models;
using YASudoku.Models.PuzzleGenerators;
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

    private SwitchPenAndPencilCmd? switchPenAndPencilCmd;
    private PressNumberCmd? pressNumberCmd;
    private SelectEraserCmd? selectEraserCmd;
    private SelectCellCmd? selectCellCmd;
    private NewGameCmd? newGameCmd;
    private RestartGameCmd? restartGameCmd;
    private PauseGameCmd? pauseGameCmd;
    private SettingsCmd? settingsCmd;
    private UndoCmd? undoCmd;

    public VisualStatesHandler? VisualState { get; private set; }

    private readonly BehaviorSubject<bool> isAnimationRunningSubject = new( false );
    public IObservable<bool> IsAnimationRunning => isAnimationRunningSubject.AsObservable();

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

        switchPenAndPencilCmd = new( VisualState );
        pressNumberCmd = new( VisualState );
        selectCellCmd = new( VisualState );
        selectEraserCmd = new( VisualState );
        newGameCmd = new( VisualState, generator, journal );
        restartGameCmd = new( VisualState, journal );
        pauseGameCmd = new( VisualState );
        settingsCmd = new( VisualState.SettingsVS );
        undoCmd = new( journal, VisualState );

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

    public void OnAnimationStarted() => isAnimationRunningSubject.OnNext( true );

    public void OnAnimationEnded( AnimationTypes animationType )
    {
        if ( animationType == AnimationTypes.AbortingGame ) {
            VisualState?.WipingGameBoardCompleted.OnNext( true );
            return; // Aborting game animation is a composite animation, so we don't want to set the animation running to false
        }

        else if ( animationType == AnimationTypes.NewGame ) {
            VisualState?.StartingNewGameCompleted.OnNext( true );
        }

        isAnimationRunningSubject.OnNext( false );
    }

    [RelayCommand]
    public async Task StartNewGame()
    {
        if ( newGameCmd == null ) return;
        bool abortingGame = VisualState?.CurrentGameState == GameStates.Running;
        await ExecuteAsyncIfNotRunningAnimation( () => newGameCmd.NewGame( abortingGame ) );
    }

    [RelayCommand]
    public async Task RestartGame()
    {
        if ( restartGameCmd == null ) return;

        bool abortingGame = VisualState?.CurrentGameState == GameStates.Running;
        await ExecuteAsyncIfNotRunningAnimation( () => restartGameCmd.RestartGame( abortingGame ) );
    }

    [RelayCommand]
    public void PauseGame() => ExecuteIfNotInRunningAnimation( () => pauseGameCmd?.TogglePausedState() );

    [RelayCommand]
    public void SwitchPenAndPencil()
        => ExecuteIfNotInRunningAnimation( () => switchPenAndPencilCmd?.SwitchPenAndPencil() );

    [RelayCommand]
    public void PressNumber( int pressedNumber )
        => ExecuteIfNotInRunningAnimation( () => pressNumberCmd?.PressNumber( pressedNumber ) );

    [RelayCommand]
    public void SelectEraser() => ExecuteIfNotInRunningAnimation( () => selectEraserCmd?.SelectEraser() );

    [RelayCommand]
    public void SelectCell( int selectedCellIndex )
        => ExecuteIfNotInRunningAnimation( () => selectCellCmd?.SelectCell( selectedCellIndex ) );

    [RelayCommand]
    public void OpenSettings() => ExecuteIfNotInRunningAnimation( () => settingsCmd?.OpenSettings() );

    [RelayCommand]
    public void UndoLastAction() => ExecuteIfNotInRunningAnimation( () => undoCmd?.UndoLastActionInJournal() );

    private async void ExecuteIfNotInRunningAnimation( Action action )
    {
        if ( await IsAnimationRunning.FirstAsync() ) return;

        action();
    }

    private async Task ExecuteAsyncIfNotRunningAnimation( Func<Task> func )
    {
        if ( await IsAnimationRunning.FirstAsync() ) return;
        await func();
    }

    public void OnPageAppearing() => VisualState?.StartGame();
}
