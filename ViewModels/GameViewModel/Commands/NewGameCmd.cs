using YASudoku.Models;
using YASudoku.Models.PuzzleGenerators;
using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class NewGameCmd
{
    private readonly IPuzzleGenerator generator;
    private readonly IPlayerJournalingService journal;
    private readonly VisualStatesHandler visualState;

    public NewGameCmd( VisualStatesHandler visualState, IPuzzleGenerator puzzleGenerator,
        IPlayerJournalingService journalingService )
    {
        this.visualState = visualState;
        journal = journalingService;
        generator = puzzleGenerator;
    }

    public void NewGame()
    {
        visualState.TimerVS.StopTimer();

        visualState.ResetVisualStatesToDefault();

        journal.ClearJournal();

        GameDataContainer newGameData = generator.GenerateNewPuzzle();
        visualState.GameData.ReplaceCollection( newGameData.AllCells );
        visualState.UpdateAllButtonRemainingCounts();

        visualState.PrepareUIForNewGame();
    }
}
