using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands.SelectCellCmdTests;

public class SelectCellWithCorrectValueTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumber_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickCellWithCorrectValueWithSameNumber_RemovesValue_KeepsNumberSelected_HighlightsSameValues()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertNumberIsSelected( EnabledNumber );
        AssertCellHasNoValue( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickCellWithCorrectValueWithDifferentNumber_ChangesValue_KeepsNumberSelected_HighlightsSameValues()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfDifferentEnabledNumber( out _ );
        // Assert
        AssertNumberIsSelected( EnabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickCellWithCorrectValueOfSameValue_RemovesValue_EnablesNumber_KeepsNumberSelected_HighlightsSameValues()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfDisabledNumber( out _ );
        // Assert
        AssertNumberIsEnabled( DisabledNumber );
        AssertNumberIsSelected( DisabledNumber );
        AssertCellHasNoValue( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickCellWithCorrectValueOfEnabledNumber_KeepsCellValue_DeselectsNumber_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithNoActiveCell_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEraser_ClickCellWithCorrectValue_RemovesValue_KeepsEraserSelected_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateEraser();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertEraserIsSelected();
        AssertNoCellIsSelected();
        AssertCellHasNoValue( affectedCell );
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencil_ClickCellWithCorrectValue_KeepsPencilSelected_SelectsCell_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickCellWithCorrectValueWithSameNumber_DeselectsNumber_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickCellWithCorrectValueWithDifferentNumber_DeselectsNumber_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfDifferentEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickCellWithCorrectValueOfSameNumber_DeselectsNumber_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickCellWithCorrectValueOfEnabledNumber_DeselectsNumber_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEraser_ClickCellWithCorrectValue_RemovesCellValue_KeepsPencilActive_KeepsEraserActive_NothingIsHighlighted()
    {
        // Arrange
        ActivatePencil();
        ActivateEraser();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertEraserIsSelected();
        AssertNoCellIsSelected();
        AssertCellHasNoValue( affectedCell );
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickDifferentCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfDifferentEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateEmptyCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveLockedCell_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateLockedCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEmptyCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateLockedCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickCellWithCorrectValue_DeselectsCell_KeepsPencilActive_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }
}
