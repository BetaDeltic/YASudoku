using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class SelectEraserCmd
{
    private readonly VisualStatesHandler visualState;

    public SelectEraserCmd( VisualStatesHandler visualState )
    {
        this.visualState = visualState;
    }

    public void SelectEraser()
    {
        if ( visualState.IsPaused ) {
            return;
        }

        if ( visualState.PencilVS.IsActive ) {
            visualState.PencilVS.DeactivateButton();
        }

        // Can't have eraser active along with input
        if ( visualState.NumPadVS.SelectedButton != null ) {
            visualState.NumPadVS.DeselectCurrentNumber();
            visualState.GameGridVS.UnhighlightCellsWithSameNumber();
        }

        // User is trying to deactivate the eraser
        if ( visualState.IsEraserActive ) {
            visualState.EraserVS.DeactivateButton();
            return;
        }

        // User is trying to activate the eraser
        if ( !visualState.GameGridVS.IsAnyCellSelected() ) {
            visualState.EraserVS.ActivateButton();
            return;
        }

        // User is trying to delete cell candidates
        if ( !visualState.SelectedCell!.HasUserFacingValue ) {
            visualState.SelectedCell.RemoveAllCandidates();
            visualState.GameGridVS.DeselectCell();
            return;
        }

        // User is trying to delete value from selected cell
        visualState.GameGridVS.RemoveValueFromSelectedCell();
        visualState.GameGridVS.HighlightSelectedCell();
    }
}
