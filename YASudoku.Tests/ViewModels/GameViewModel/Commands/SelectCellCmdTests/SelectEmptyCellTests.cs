using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands.SelectCellCmdTests;

public class SelectEmptyCellTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumber_ClickEmptyCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithNoActiveCell_ClickEmptyCell_SelectsCell_HighlightCellAndRelatedCells()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickSameEmptyCell_DeselectsCell_UnhighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateEmptyCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        // Assert
        AssertNoCellIsSelected();
        AssertCellIsNotSelectedOrHighlighted( affectedCell );
        AssertRelatedCellsAreNotHighlighted( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickEmptyCell_FillsCellWithValue_KeepsNumberSelected_DeselectsCell_HighlightsAllCellsWithSameNumber()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        // Assert
        AssertCellIsFilledWithSpecificValue( affectedCell, EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertNumberIsEnabled( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickDifferentEmptyCell_DeselectsPreviousCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateEmptyCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickDifferentEmptyCell();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickEmptyCell_DeselectsNumber_HighlightCellAndRelatedCells()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        // Assert
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEraser_ClickEmptyCell_JournalNotCalled_NoCellSelected_NoCellHighlighted_KeepsEraserActive()
    {
        // Arrange
        ActivateEraser();
        // Act
        ClickEmptyCell( out _ );
        // Assert
        AssertNoTransactionAddedToJournal();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
        AssertEraserIsSelected();
    }

    [Fact]
    public void WithActivePencil_ClickEmptyCell_SelectsCell_HighlightsCellAndRelatedCells_KeepsPencilActive()
    {
        // Arrange
        ActivatePencil();
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
        AssertPencilIsSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickEmptyCell_AddsCandidate_KeepsNumberSelected_DeselectsCell_HighlightsAllCellsWithSameNumber_KeepsPencilActive()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        // Assert
        AssertCellContainsCandidate( affectedCell, EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertNumberIsEnabled( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
        AssertPencilIsSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickEmptyCell_AddsCandidate_KeepsNumberSelected_DeselectsCell_HighlightsAllCellsWithSameNumber_KeepsPencilActive()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell( out _ );
        // Assert
        AssertCellContainsCandidate( affectedCell, DisabledNumber );
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsDisabled( DisabledNumber );
        AssertNoCellIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
        AssertPencilIsSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateEmptyCell( out _ );
        // Act
        ClickEmptyCell( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveEmptyCell_ClickDifferentEmptyCell_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateEmptyCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickDifferentEmptyCell();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }
}
