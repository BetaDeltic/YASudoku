using YASudoku.Common;
using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class SelectCellCmd
{
    private readonly CommonButtonVisualState pencil;
    private readonly CommonButtonVisualState eraser;
    private readonly NumPadVisualState numPad;
    private readonly GameGridVisualState grid;
    private readonly VisualStatesHandler visualState;
    private readonly IPlayerJournalingService journalingService;

    private NumPadButton? SelectedNumber => numPad.SelectedButton;
    private GameGridCellVisualData? SelectedCell => grid.SelectedCell;

    private int SelectedButtonNumber => numPad.SelectedButtonNumber;

    public SelectCellCmd( VisualStatesHandler visualState, IPlayerJournalingService journalingService )
    {
        pencil = visualState.PencilVS;
        eraser = visualState.EraserVS;
        numPad = visualState.NumPadVS;
        grid = visualState.GameGridVS;
        this.visualState = visualState;
        this.journalingService = journalingService;
    }

    public void SelectCell( int newSelectedCellIndex )
    {
        if ( visualState.IsPaused ) {
            return;
        }

        // User is deselecting previous cell
        if ( grid.IsCellSelected( newSelectedCellIndex ) ) {
            grid.UnhighlightCellsWithSameNumber();
            grid.DeselectCell();
            return;
        }

        if ( numPad.IsAnyNumberSelected() ) {
            UserIsTryingToChangeCellValue( newSelectedCellIndex );
            return;
        }

        if ( eraser.IsActive ) {
            UserIsTryingToChangeCellWithEraser( newSelectedCellIndex );
            return;
        }

        // User is only selecting different cell
        grid.UnhighlightCellsWithSameNumber();
        grid.SelectNewCell( newSelectedCellIndex );
    }

    private void UserIsTryingToChangeCellValue( int newSelectedCellIndex )
    {
        grid.SelectNewCell( newSelectedCellIndex );

        if ( SelectedCell!.IsLockedForChanges ) {
            numPad.DeselectCurrentNumber();
            grid.HighlightSelectedCell();
            grid.HighlightCellsWithSameNumber( SelectedCell.UserFacingValue );
            return;
        }

        if ( pencil.IsActive ) {
            UserIsTryingToChangeCellCandidates();
            return;
        }

        // User is erasing current cell value using same button
        if ( SelectedCell.UserFacingValue == SelectedButtonNumber ) {
            journalingService.AddTransaction( PlayerTransactionTypes.CellValueErased, SelectedCell, SelectedButtonNumber );
            grid.RemoveValueFromSelectedCell();
            SelectedCell.ShowCandidates();
            grid.DeselectCell();
            grid.HighlightCellsWithSameNumber( SelectedButtonNumber );
            return;
        }

        // User is trying to use disabled button for something else than removing
        if ( !SelectedNumber!.IsEnabled ) {
            numPad.DeselectCurrentNumber();
            grid.DeselectCell();
            grid.UnhighlightCellsWithSameNumber();
            return;
        }

        // User wants to change selected cell to selected number
        if ( SelectedCell.HasUserFacingValue ) {
            journalingService.AddTransaction( PlayerTransactionTypes.CellValueChanged, SelectedCell, SelectedCell.UserFacingValue );
        } else {
            journalingService.AddTransaction( PlayerTransactionTypes.CellValueAdded, SelectedCell, SelectedButtonNumber );
        }

        grid.ChangeSelectedCellValueAndNotify( SelectedButtonNumber, out var affectedCells );
        affectedCells.ForEach( cell =>
            journalingService.AddTransaction( PlayerTransactionTypes.RelatedCandidateRemoved, cell, SelectedButtonNumber )
        );

        grid.DeselectCell();
        grid.HighlightCellsWithSameNumber( SelectedButtonNumber );
    }

    private void UserIsTryingToChangeCellCandidates()
    {
        if ( SelectedCell!.HasUserFacingValue )
            return;

        // User wants to remove value from candidates
        if ( SelectedCell.HasNumberAsCandidate( SelectedButtonNumber ) ) {
            journalingService.AddTransaction( PlayerTransactionTypes.CandidateRemoved, SelectedCell, SelectedButtonNumber );
            SelectedCell.ChangeCandidateVisibility( SelectedButtonNumber, false );
            grid.DeselectCell();
            grid.HighlightCellsWithSameNumber( SelectedButtonNumber );
            return;
        }

        // User wants to add selected number to selected cell candidates
        journalingService.AddTransaction( PlayerTransactionTypes.CandidateAdded, SelectedCell, SelectedButtonNumber );
        SelectedCell.ChangeCandidateVisibility( SelectedButtonNumber, true );
        SelectedCell.ShowCandidates();
        grid.DeselectCell();
        grid.HighlightCellsWithSameNumber( SelectedButtonNumber );
    }

    private void UserIsTryingToChangeCellWithEraser( int newSelectedCellIndex )
    {
        grid.SelectNewCell( newSelectedCellIndex );
        // User is erasing all the candidates
        if ( !SelectedCell!.HasUserFacingValue ) {
            IEnumerable<int> cellCandidates = SelectedCell.GetAllCandidateValues();
            cellCandidates.ForEach( candidate =>
                journalingService
                .AddTransaction( PlayerTransactionTypes.CandidateRemoved, SelectedCell, candidate )
            );

            SelectedCell.HideAllCandidates();
            grid.DeselectCell();
            return;
        }

        // User is erasing cell value
        journalingService.AddTransaction( PlayerTransactionTypes.CellValueErased, SelectedCell, SelectedCell.UserFacingValue );
        grid.RemoveValueFromSelectedCell();
        grid.DeselectCell();
    }
}
