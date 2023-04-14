using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class SelectCellCmd : CommandsBase
{
    private int SelectedButtonNumber => numPad.SelectedButtonNumber;

    public SelectCellCmd( VisualStatesHandler visualState ) : base( visualState ) { }

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
            UserIsTryingToChangeCellValueWithSelectedNumber( newSelectedCellIndex );
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

    private void UserIsTryingToChangeCellValueWithSelectedNumber( int newSelectedCellIndex )
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
            grid.RemoveValueFromSelectedCell();
            SelectedCell.DisplayCandidates();
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
        grid.ChangeSelectedCellValueAndNotify( SelectedButtonNumber );

        grid.DeselectCell();
        grid.HighlightCellsWithSameNumber( SelectedButtonNumber );
    }

    private void UserIsTryingToChangeCellCandidates()
    {
        if ( SelectedCell!.HasUserFacingValue )
            return;

        // User wants to remove value from candidates
        if ( SelectedCell.HasNumberAsCandidate( SelectedButtonNumber ) ) {
            SelectedCell.RemoveCandidate( SelectedButtonNumber );
            grid.DeselectCell();
            grid.HighlightCellsWithSameNumber( SelectedButtonNumber );
            return;
        }

        // User wants to add selected number to selected cell candidates
        SelectedCell.AddCandidate( SelectedButtonNumber );
        SelectedCell.DisplayCandidates();
        grid.DeselectCell();
        grid.HighlightCellsWithSameNumber( SelectedButtonNumber );
    }

    private void UserIsTryingToChangeCellWithEraser( int newSelectedCellIndex )
    {
        grid.SelectNewCell( newSelectedCellIndex );
        // User is erasing all the candidates
        if ( !SelectedCell!.HasUserFacingValue ) {
            SelectedCell.RemoveAllCandidates();
            grid.DeselectCell();
            return;
        }

        // User is erasing cell value
        grid.RemoveValueFromSelectedCell();
        grid.DeselectCell();
    }
}
