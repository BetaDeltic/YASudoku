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
    public event Action? NewGameAfterFinishedOne;

    public readonly int gridSize = 9;

    private readonly IPuzzleGenerator generator;
    private readonly ISettingsService settings;
    private readonly IPlayerJournalingService journalingService;
    public readonly IServiceProvider serviceProvider;

    public VisualStatesHandler? VisualState { get; private set; }

    private SwitchPenAndPencilCmd? switchPenAndPencilCmd;
    private PressNumberCmd? pressNumberCmd;
    private SelectEraserCmd? selectEraserCmd;
    private SelectCellCmd? selectCellCmd;

    public GameVM( IPuzzleGenerator puzzleGenerator, ISettingsService settingsService, IServiceProvider serviceProvider, IPlayerJournalingService journalingService )
        : base( settingsService )
    {
        settings = settingsService;
        this.serviceProvider = serviceProvider;
        this.journalingService = journalingService;
        generator = puzzleGenerator;
    }

    public void Dispose()
    {
        VisualState?.Dispose();
        GC.SuppressFinalize( this );
    }

    [RelayCommand]
    public void StartNewGame()
    {
        if ( VisualState == null ) return;

        VisualState.StopTimer();

        ResetVisualStatesToDefault();

        GameDataContainer newGameData = generator.GenerateNewPuzzle();
        VisualState.GameData.ReplaceCollection( newGameData.AllCells );
        VisualState.UpdateAllButtonRemainingCounts();

        PrepareUIForNewGame();
    }

    [RelayCommand]
    public void RestartGame()
    {
        if ( VisualState == null ) return;

        ResetVisualStatesToDefault();

        VisualState.GameData.RestartCellValues();

        VisualState.UpdateAllButtonRemainingCounts();

        PrepareUIForNewGame();
    }

    [RelayCommand]
    public void PauseGame()
    {
        if ( VisualState == null ) return;

        // User is trying to unpause the game
        if ( VisualState.IsPaused ) {
            VisualState.PauseVS.DeactivateButton();
            VisualState.GameGridVS.ShowAllCellValues();
            VisualState.UnpauseTimer();
            // User is trying to pause the game
        } else {
            VisualState.StopTimer();
            VisualState.PauseVS.ActivateButton();
            VisualState.NumPadVS.DeselectCurrentNumber();
            VisualState.GameGridVS.DeselectCell();
            VisualState.GameGridVS.UnhighlightCellsWithSameNumber();
            VisualState.GameGridVS.HideAllCellValues();
        }
    }

    [RelayCommand]
    public void SwitchPenAndPencil() => switchPenAndPencilCmd?.SwitchPenAndPencil();

    [RelayCommand]
    public void PressNumber( int pressedNumber ) => pressNumberCmd?.PressNumber( pressedNumber );

    [RelayCommand]
    public void SelectEraser() => selectEraserCmd?.SelectEraser();

    [RelayCommand]
    public void SelectCell( int selectedCellIndex ) => selectCellCmd?.SelectCell( selectedCellIndex );

    [RelayCommand]
    public void OpenSettings()
    {
        if ( VisualState == null ) return;

        VisualState.AreSettingsVisible = true;
    }

    [RelayCommand]
    public void UndoLastAction() => journalingService.ReverseTransaction();

    public void PrepareGameView( bool generateNew )
    {
        GameDataContainer gameData = GetPuzzleData( generateNew );
        GameGridVisualDataCollection visualData = new( gameData.AllCells, gridSize, settings );

        VisualState = new( gridSize, visualData, serviceProvider );
        
        journalingService.SetVisualState( VisualState );

        switchPenAndPencilCmd = new( VisualState.PencilVS );
        pressNumberCmd = new( VisualState, journalingService );
        selectCellCmd = new( VisualState, journalingService );
        selectEraserCmd = new( VisualState, journalingService );

        VisualState.UpdateAllButtonRemainingCounts();
    }

    public void OnPageAppearing() => VisualState?.StartGame();

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

    private void ResetVisualStatesToDefault()
    {
        if ( VisualState!.IsPaused ) {
            VisualState.GameGridVS.ShowAllCellValues();
            VisualState.PauseVS.DeactivateButton();
        }

        VisualState.NumPadVS.DeselectCurrentNumber();
        VisualState.GameGridVS.DeselectCell();
        VisualState.GameGridVS.UnhighlightCellsWithSameNumber();

        if ( VisualState.PencilVS.IsActive )
            VisualState.PencilVS.DeactivateButton();

        journalingService.ClearJournal();
    }

    private void PrepareUIForNewGame()
    {
        if ( VisualState!.CurrentVisualState == VisualStates.VisualStates.Finished ) {
            NewGameAfterFinishedOne?.Invoke();
        }

        VisualState.StartGame();
    }
}
