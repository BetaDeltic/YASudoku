using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class UndoCmdTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumberOrCell_ClickUndo_NothingHappens()
    {
        // Arrange
        // Act
        ClickUndoButton();
        // Assert
        AssertNoCellIsSelected();
        AssertNoNumberIsActive();
        AssertNoNumberIsSelected();
        AssertNoCellIsHighlighted();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickUndo_NumberStaysSelected()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertNumberIsSelected( EnabledNumber );
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickUndo_NumberStaysSelected()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertNumberIsSelected( DisabledNumber );
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActiveEmptyCell_ClickUndo_CellStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateEmptyCell( out _ );
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActiveLockedCell_ClickUndo_CellStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateLockedCell( out _ );
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickUndo_CellStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickUndo_CellStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickUndo_CellStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActiveEraser_ClickUndo_EraserStaysSelected()
    {
        // Arrange
        ActivateEraser();
        // Act
        ClickUndoButton();
        // Assert
        AssertEraserIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActivePencil_ClickUndo_PencilStaysSelected()
    {
        // Arrange
        ActivatePencil();
        // Act
        ClickUndoButton();
        // Assert
        AssertPencilIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickUndo_NumberStaysSelected_PencilStaysSelected()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertNumberIsSelected( EnabledNumber );
        AssertPencilIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickUndo_NumberStaysSelected_PencilStaysSelected()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertNumberIsSelected( DisabledNumber );
        AssertPencilIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickUndo_CellStaysSelected_PencilStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateEmptyCell( out _ );
        ActivatePencil();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertPencilIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickUndo_CellStaysSelected_PencilStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateLockedCell( out _ );
        ActivatePencil();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertPencilIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickUndo_CellStaysSelected_PencilStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        ActivatePencil();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertPencilIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickUndo_CellStaysSelected_PencilStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        ActivatePencil();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertPencilIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickUndo_CellStaysSelected_PencilStaysSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        ActivatePencil();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertPencilIsSelected();
        AssertNoTransactionAddedToJournal();
    }

    [Fact]
    public void HavingFilledCellWithValue_ClickUndo_CellValueIsReverted()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ClickEmptyCell( out int originalValue );
        ActivateEnabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void HavingFilledCellWithValueThatRemovedRelatedCandidates_ClickUndo_CandidatesAreRestored()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ClickRelatedEmptyCell();
        ActivatePencil();
        ActivateEnabledNumber();
        ActivateEmptyCell( out _ );
        ActivateEnabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellContainsCandidate( affectedCell, EnabledNumber );
    }

    [Fact]
    public void HavingChangedCellValue_ClickUndo_CellValueIsReverted()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfDifferentEnabledNumber( out int originalValue );
        ActivateEnabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void HavingErasedCellValue_ClickUndo_CellValueIsReverted()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        ActivateEraser();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void HavingAddedCellCandidate_ClickUndo_CandidateIsRemoved()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellDoesNotHaveSpecificCandidate( affectedCell, EnabledNumber );
    }

    [Fact]
    public void HavingRemovedCellCandidate_ClickUndo_CellCandidateIsReAdded()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        ClickUndoButton();
        // Assert
        AssertCellContainsCandidate( affectedCell, EnabledNumber );
    }
}
