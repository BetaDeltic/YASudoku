using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class RestartGameCmd : GameCommandsBase
{
    public RestartGameCmd( VisualStatesHandler visualState, IPlayerJournalingService journalingService )
        : base( visualState, journalingService ) { }

    public async void RestartGame( bool previousGameAborted )
    {
        if ( previousGameAborted ) await EndRunningGame();

        visualState.GameData.RestartCellValues();
        visualState.UpdateAllButtonRemainingCounts();

        await StartNewGame();
    }
}
