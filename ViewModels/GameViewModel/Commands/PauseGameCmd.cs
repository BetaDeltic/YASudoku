using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class PauseGameCmd
{
    private readonly VisualStatesHandler visualState;

    public PauseGameCmd( VisualStatesHandler visualState )
    {
        this.visualState = visualState;
    }

    public void TogglePausedState()
    {
        bool pause = !visualState.IsPaused;

        if ( pause ) {
            visualState.TimerVS.PauseTimer();
            visualState.PauseVS.ActivateButton();
            visualState.NumPadVS.DeselectCurrentNumber();
            visualState.GameGridVS.DeselectCell();
            visualState.GameGridVS.UnhighlightCellsWithSameNumber();
            visualState.GameGridVS.HideAllCellValues();
        } else {
            visualState.PauseVS.DeactivateButton();
            visualState.GameGridVS.ShowAllCellValues();
            visualState.TimerVS.UnpauseTimer();
        }
    }
}
