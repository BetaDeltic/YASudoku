using YASudoku.Models;
using YASudoku.Models.PuzzleGenerators;
using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class NewGameCmd : GameCommandsBase
{
    private readonly IPuzzleGenerator generator;

    public NewGameCmd( VisualStatesHandler visualState, IPuzzleGenerator puzzleGenerator,
        IPlayerJournalingService journalingService ) : base( visualState, journalingService )
    {
        generator = puzzleGenerator; 
    }

    public async void NewGame( bool previousGameAborted )
    {
        if ( previousGameAborted ) await EndRunningGame();
        await GenerateAndReplaceGameData();
        await StartNewGame();
    }

    public async Task GenerateAndReplaceGameData()
    {
        GameDataContainer newGameData = await Task.Run( () => generator.GenerateNewPuzzle() );
        visualState.GameData.ReplaceCollection( newGameData.AllCells );
        visualState.UpdateAllButtonRemainingCounts();
    }
}
