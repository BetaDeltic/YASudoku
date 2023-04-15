using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class PressNumberCmd : CommandsBase
{
    public PressNumberCmd( VisualStatesHandler visualState ) : base( visualState ) { }

    public void PressNumber( int pressedNumber )
    {
        if ( IsPaused ) {
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
        if ( SelectedCell == null ) return;

        // User is trying to change locked cell
        if ( SelectedCell.IsLockedForChanges ) {
            grid.UnhighlightCellsWithSameNumber();
            grid.DeselectCell();
            numPad.SelectNewNumber( pressedNumber );
            return;
        }

        numPad.SelectNewNumber( pressedNumber );
        if ( SelectedNumber == null ) return;

        if ( pencil.IsActive ) {
            UserIsTryingToChangeCellCandidates( pressedNumber );
            return;
        }

        // User wants to remove value from cell
        if ( SelectedCell.UserFacingValue == pressedNumber ) {
            grid.RemoveValueFromSelectedCell();
            grid.HighlightSelectedCell();
            DeselectCurrentNumberAndUnhighlightSameCells();
            return;
        }

        // User is trying to use disabled number for something else than removal
        if ( !SelectedNumber.IsEnabled ) {
            DeselectCurrentNumberAndUnhighlightSameCells();
            grid.DeselectCell();
            return;
        }

        // User wants to set selected cell to selected number
        grid.ChangeSelectedCellValueAndNotify( pressedNumber );

        grid.HighlightSelectedCell();
        grid.HighlightCellsWithSameNumber( pressedNumber );
        numPad.DeselectCurrentNumber();
    }

    private void UserIsTryingToChangeCellCandidates( int pressedNumber )
    {
        if ( SelectedCell == null ) return;

        // User is trying to change candidates of solved cell
        if ( SelectedCell.HasUserFacingValue ) {
            grid.DeselectCell();
            grid.HighlightCellsWithSameNumber( pressedNumber );
            return;
        }

        // User wants to remove value from candidates
        if ( SelectedCell.HasNumberAsCandidate( pressedNumber ) ) {
            SelectedCell.RemoveCandidate( pressedNumber );
            DeselectCurrentNumberAndUnhighlightSameCells();
            grid.HighlightSelectedCell();
            return;
        }

        // User wants to add selected number to selected cell candidates
        SelectedCell.AddCandidate( pressedNumber );
        SelectedCell.DisplayCandidates();
        DeselectCurrentNumberAndUnhighlightSameCells();
        grid.HighlightSelectedCell();
    }

    private void DeselectCurrentNumberAndUnhighlightSameCells()
    {
        grid.UnhighlightCellsWithSameNumber( numPad.SelectedButtonNumber );
        numPad.DeselectCurrentNumber();
    }
}
