using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands.SelectCellCmdTests;

public class SelectLockedCellTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumber_ClickLockedCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickLockedCell_DeselectsNumber_SelectsLockedCell_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out int originalValue );
        // Assert
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickLockedCell_DeselectsNumber_SelectsLockedCell_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out int originalValue );
        // Assert
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void WithNoActiveCell_ClickLockedCell_SelectsCell_HighlightsAndRelatedCells_HighlightCellsWithSameValue()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencil_ClickLockedCell_SelectsCell_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue_KeepsPencilActive()
    {
        // Arrange
        ActivatePencil();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out int originalValue );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertPencilIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickLockedCell_SelectsCell_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue_KeepsPencilActive_DeselectsNumber()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out int originalValue );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickLockedCell_SelectsCell_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue_KeepsPencilActive_DeselectsNumber()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out int originalValue );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEraser_ClickLockedCell_NoCellSelected_KeepsCellValue_KeepsPencilActive_KeepsEraserActive()
    {
        // Arrange
        ActivatePencil();
        ActivateEraser();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out int originalValue );
        // Assert
        AssertNoCellIsSelected();
        AssertPencilIsSelected();
        AssertEraserIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void WithActiveEraser_ClickLockedCell_NoCellSelected_KeepsCellValue_KeepsEraserActive()
    {
        // Arrange
        ActivateEraser();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out int originalValue );
        // Assert
        AssertNoCellIsSelected();
        AssertEraserIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
    }

    [Fact]
    public void WithActiveLockedCell_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateLockedCell();
        // Act
        ClickLockedCell( out _ );
        // Assert
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateLockedCell();
        // Act
        ClickLockedCell( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveLockedCell_ClickDifferentLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateLockedCell();
        // Act
        GameGridCellVisualData affectedCell = ClickDifferentLockedCell();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateEmptyCell();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEmptyCell();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void
        WithActivePencilAndWithActiveLockedCell_ClickLockedCell_DeselectsCell_KeepsPencilActive_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateLockedCell();
        // Act
        ClickLockedCell( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickLockedCell_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickLockedCell( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }
}
