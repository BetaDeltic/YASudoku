using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class RestartGameCmd
{
    private readonly VisualStatesHandler visualState;

    public RestartGameCmd( VisualStatesHandler visualState )
    {
        this.visualState = visualState;
    }

    public void RestartGame()
    {
        visualState.ResetVisualStatesToDefault();

        visualState.GameData.RestartCellValues();

        visualState.UpdateAllButtonRemainingCounts();

        visualState.PrepareUIForNewGame();
    }
}
