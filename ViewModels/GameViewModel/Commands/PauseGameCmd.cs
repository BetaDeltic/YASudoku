using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class PauseGameCmd : CommandsBase
{
    private readonly TimerVisualState timerVS;
    private readonly CommonButtonVisualState pauseVS;

    public PauseGameCmd( VisualStatesHandler visualState ) : base( visualState )
    {
        timerVS = visualState.TimerVS;
        pauseVS = visualState.PauseVS;
    }

    public void TogglePausedState()
    {
        bool pause = !IsPaused;

        if ( pause ) {
            timerVS.PauseTimer();
            pauseVS.ActivateButton();
            grid.UnhighlightCellsWithSameNumber();
            grid.UnhiglightSelectedCellAndRelatedCells();
            grid.HideAllCellValues();
        } else {
            pauseVS.DeactivateButton();
            grid.ShowAllCellValues();
            if ( SelectedCell != null ) {
                grid.HighlightSelectedCell();
                grid.HighlightCellsWithSameNumber( SelectedCell.UserFacingValue );
            }
            if ( SelectedNumber != null ) {
                grid.HighlightCellsWithSameNumber( visualState.NumPadVS.SelectedButtonNumber );
            }
            timerVS.UnpauseTimer();
        }
    }
}
