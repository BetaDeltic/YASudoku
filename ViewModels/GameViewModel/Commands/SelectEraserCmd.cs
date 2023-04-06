using YASudoku.Common;
using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public partial class SelectEraserCmd
{
    private readonly VisualStatesHandler visualState;
    private readonly IPlayerJournalingService journalingService;

    public SelectEraserCmd( VisualStatesHandler visualState, IPlayerJournalingService journalingService )
    {
        this.visualState = visualState;
        this.journalingService = journalingService;
    }

    public void SelectEraser()
    {
        if ( visualState.IsPaused ) {
            return;
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
            IEnumerable<int> cellCandidates = visualState.SelectedCell.GetAllCandidateValues();
            cellCandidates.ForEach( candidate =>
                journalingService
                .AddTransaction( PlayerTransactionTypes.CandidateRemoved, visualState.SelectedCell, candidate )
            );

            visualState.SelectedCell.HideAllCandidates();
            visualState.GameGridVS.DeselectCell();
            return;
        }

        // User is trying to delete value from selected cell
        journalingService.AddTransaction(
            PlayerTransactionTypes.CellValueErased, visualState.SelectedCell, visualState.SelectedCell.UserFacingValue
        );
        visualState.GameGridVS.RemoveValueFromSelectedCell();
        visualState.GameGridVS.HighlightSelectedCell();
    }
}
