using YASudoku.Common;
using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class PressNumberCmd
{
    private readonly CommonButtonVisualState pencil;
    private readonly CommonButtonVisualState eraser;
    private readonly NumPadVisualState numPad;
    private readonly GameGridVisualState grid;
    private readonly VisualStatesHandler visualState;
    private readonly IPlayerJournalingService journalingService;

    private GameGridCellVisualData? SelectedCell => grid.SelectedCell;
    private NumPadButton? SelectedNumber => numPad.SelectedButton;

    public PressNumberCmd( VisualStatesHandler visualState, IPlayerJournalingService journalingService )
    {
        pencil = visualState.PencilVS;
        eraser = visualState.EraserVS;
        numPad = visualState.NumPadVS;
        grid = visualState.GameGridVS;
        this.visualState = visualState;
        this.journalingService = journalingService;
    }

    public void PressNumber( int pressedNumber )
    {
        if ( visualState.IsPaused ) {
            return;
        }

        // Eraser can't be enabled simultaneously with input
        eraser.DeactivateButton();

        // User is de-selecting previously selected number
        if ( numPad.IsNumberSelected( pressedNumber ) ) {
            DeselectCurrentNumberAndUnhighlightSameCells();
            return;
        }

        // User is selecting different number
        if ( numPad.IsAnyNumberSelected() ) {
            grid.DeselectCell();
            grid.UnhighlightCellsWithSameNumber();
            DeselectCurrentNumberAndUnhighlightSameCells();
            numPad.SelectNewNumber( pressedNumber );
            return;
        }

        // User is only selecting a new number
        if ( !grid.IsAnyCellSelected() ) {
            numPad.SelectNewNumber( pressedNumber );
            return;
        }

        UserIsTryingToChangeSelectedCellWithSelectedNumber( pressedNumber );
    }

    private void UserIsTryingToChangeSelectedCellWithSelectedNumber( int pressedNumber )
    {
        // User is trying to change locked cell
        if ( SelectedCell!.IsLockedForChanges ) {
            grid.UnhighlightCellsWithSameNumber();
            grid.DeselectCell();
            numPad.SelectNewNumber( pressedNumber );
            return;
        }
        numPad.SelectNewNumber( pressedNumber );

        // User wants to remove value from cell
        if ( SelectedCell.UserFacingValue == pressedNumber ) {
            journalingService.AddTransaction( PlayerTransactionTypes.CellValueErased, SelectedCell, pressedNumber );
            grid.RemoveValueFromSelectedCell();
            grid.HighlightSelectedCell();
            DeselectCurrentNumberAndUnhighlightSameCells();
            return;
        }

        if ( pencil.IsActive ) {
            UserIsTryingToChangeCellCandidates( pressedNumber );
            return;
        }

        // User is trying to use disabled number for something else than removal
        if ( !SelectedNumber!.IsEnabled ) {
            DeselectCurrentNumberAndUnhighlightSameCells();
            grid.DeselectCell();
            return;
        }

        // User wants to set selected cell to selected number
        if ( SelectedCell.HasUserFacingValue ) {
            journalingService.AddTransaction( PlayerTransactionTypes.CellValueChanged, SelectedCell, SelectedCell.UserFacingValue );
        } else {
            journalingService.AddTransaction( PlayerTransactionTypes.CellValueAdded, SelectedCell, pressedNumber );
        }

        grid.ChangeSelectedCellValueAndNotify( pressedNumber, out var affectedCells );
        affectedCells.ForEach( cell => 
            journalingService.AddTransaction( PlayerTransactionTypes.RelatedCandidateRemoved, cell, pressedNumber )
        );

        grid.HighlightSelectedCell();
        grid.HighlightCellsWithSameNumber( pressedNumber );
        numPad.DeselectCurrentNumber();
    }

    private void UserIsTryingToChangeCellCandidates( int pressedNumber )
    {
        // User is trying to change candidates of solved cell
        if ( SelectedCell!.HasUserFacingValue ) {
            DeselectCurrentNumberAndUnhighlightSameCells();
            return;
        }

        // User wants to remove value from candidates
        if ( SelectedCell.HasNumberAsCandidate( pressedNumber ) ) {
            journalingService.AddTransaction( PlayerTransactionTypes.CandidateRemoved, SelectedCell, pressedNumber );
            SelectedCell!.ChangeCandidateVisibility( pressedNumber, false );
            grid.HighlightSelectedCell();
            DeselectCurrentNumberAndUnhighlightSameCells();
            return;
        }

        // User wants to add selected number to selected cell candidates
        journalingService.AddTransaction( PlayerTransactionTypes.CandidateAdded, SelectedCell, pressedNumber );

        SelectedCell!.ChangeCandidateVisibility( pressedNumber, true );
        SelectedCell!.ShowCandidates();
        grid.HighlightSelectedCell();
        grid.HighlightCellsWithSameNumber( pressedNumber );
        numPad.DeselectCurrentNumber();
    }

    private void DeselectCurrentNumberAndUnhighlightSameCells()
    {
        grid.UnhighlightCellsWithSameNumber( numPad.SelectedButtonNumber );
        numPad.DeselectCurrentNumber();
    }
}
