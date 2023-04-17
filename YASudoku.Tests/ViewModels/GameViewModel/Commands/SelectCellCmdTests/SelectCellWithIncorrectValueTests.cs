using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands.SelectCellCmdTests;

public class SelectCellWithIncorrectValueTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumber_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelectedOrRelated( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickCellWithIncorrectValueWithSameNumber_RemovesCellValue_KeepsNumberActive_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertNumberIsSelected( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellHasNoValue( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickCellWithIncorrectValueWithDifferentNumber_ChangesValue_KeepsNumberSelected_HighlightsSameValues()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfDifferentEnabledNumber( out int originalValue );
        // Assert
        AssertNumberIsSelected( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickCellWithIncorrectValueOfSameNumber_RemovesCellValue_EnablesNumber_KeepsNumberActive_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfDisabledNumber( out _ );
        // Assert
        AssertNumberIsSelected( DisabledNumber );
        AssertNoCellIsSelected();
        AssertCellHasNoValue( affectedCell );
        AssertNumberIsEnabled( DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickCellWithIncorrectValueOfEnabledNumber_DeselectsNumber_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertNoNumberIsSelected();
        AssertNumberIsDisabled( DisabledNumber );
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelectedOrRelated( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithNoActiveCell_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelectedOrRelated( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEraser_ClickCellWithIncorrectValue_RemovesCellValue_KeepsEraserActive_NoCellIsSelected_NothingIsHighlighted()
    {
        // Arrange
        ActivateEraser();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertEraserIsSelected();
        AssertNoCellIsSelected();
        AssertCellHasNoValue( affectedCell );
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencil_ClickCellWithIncorrectValue_KeepsCellValue_KeepsPencilActive_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelectedOrRelated( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickCellWithIncorrectValueWithSameNumber_SelectsCell_DeselectsNumber_KeepsPencilActive_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelectedOrRelated( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickCellWithIncorrectValueWithDifferentNumber_SelectsCell_DeselectsNumber_KeepsPencilActive_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfDifferentEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelectedOrRelated( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickCellWithIncorrectValueWithSameNumber_SelectsCell_DeselectsNumber_KeepsPencilActive_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfDisabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelectedOrRelated( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickCellWithIncorrectValueOfEnabledNumber_SelectsCell_DeselectsNumber_KeepsPencilActive_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelectedOrRelated( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEraser_ClickCellWithIncorrectValue_RemovesCellValue_KeepsPencilActive_KeepsEraserActive_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateEraser();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertEraserIsSelected();
        AssertNoCellIsSelected();
        AssertCellHasNoValue( affectedCell );
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickDifferentCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfDifferentEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateEmptyCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveLockedCell_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateLockedCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEmptyCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateLockedCell( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickCellWithIncorrectValue_DeselectsCell_KeepsPencilActive_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }
}
