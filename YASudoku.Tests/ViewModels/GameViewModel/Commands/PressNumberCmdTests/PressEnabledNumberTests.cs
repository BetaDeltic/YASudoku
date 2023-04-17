using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands.PressNumberCmdTests;

public class PressEnabledNumberTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumber_ClickEnabledNumber_ActivatesNumber_HighlightsOnlyCellsWithSameNumber()
    {
        // Arrange
        // Act
        SelectEnabledNumber();
        // Assert
        AssertEnabledNumberIsSelected();
        AssertEnabledNumberIsActive();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickDifferentEnabledNumber_ActivatesNumber_DeselectsCell_HighlightsOnlyCellsWithSameNumber()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNumberIsSelected( DifferentEnabledNumber );
        AssertNumberIsEnabled( DifferentEnabledNumber );
        AssertNumberIsActive( DifferentEnabledNumber );
        AssertNumberIsNotActive( EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DifferentEnabledNumber );
    }

    [Fact]
    public void WithActiveLockedCell_ClickEnabledNumberWithSameValue_KeepsCellValue_ActivatesNumber_DeselectsCell_HighlightsOnlyCellsWithSameNumber()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateLockedCellWithValueOfEnabledNumber( out int originalValue );
        // Act
        SelectEnabledNumber();
        // Assert
        AssertEnabledNumberIsSelected();
        AssertEnabledNumberIsActive();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveLockedCell_ClickEnabledNumberWithDifferentValue_KeepsCellValue_ActivatesNumber_DeselectsCell_HighlightsOnlyCellsWithSameNumber()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateLockedCellWithValueOfEnabledNumber( out int originalValue );
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNumberIsSelected( DifferentEnabledNumber );
        AssertNumberIsEnabled( DifferentEnabledNumber );
        AssertNumberIsActive( DifferentEnabledNumber );
        AssertNumberIsNotActive( EnabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DifferentEnabledNumber );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickEnabledNumberWithSameValue_FillCellValue_KeepsCellSelected_DeselectsNumber_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellIsFilledWithSpecificValue( affectedCell, EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickEnabledNumberWithDifferentValue_FillCellValue_KeepsCellSelected_DeselectsNumber_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellIsFilledWithSpecificValue( affectedCell, DifferentEnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickEnabledNumberWithSameValue_RemovesCellValue_KeepsCellSelected_DeselectsNumber_HighlightsCellAndRelatedCells()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellHasNoValue( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickEnabledNumberWithDifferentValue_FillCellValue_KeepsCellSelected_DeselectsNumber_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellIsFilledWithSpecificValue( affectedCell, DifferentEnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickEnabledNumberWithSameValue_RemovesCellValue_KeepsCellSelected_DeselectsNumber_HighlightsCellAndRelatedCells()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellHasNoValue( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickEnabledNumberWithDifferentValue_FillCellValue_KeepsCellSelected_DeselectsNumber_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellIsFilledWithSpecificValue( affectedCell, DifferentEnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEraser_ClickEnabledNumber_DeselectsEraser_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateEraser();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertEraserIsNotSelected();
        AssertNumberIsActive( EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencil_ClickEnabledNumber_KeepsPencilSelected_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsActive( EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickEnabledNumber_AddCandidate_DeselectsNumber_KeepsCellSelected_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateEmptyCell( out _ );
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellContainsCandidate( affectedCell, EnabledNumber );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickEnabledNumberWithSameValue_KeepsCellValue_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateLockedCellWithValueOfEnabledNumber( out int originalValue );
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNumberIsActive( EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickEnabledNumberWithDifferentValue_KeepsCellValue_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateLockedCellWithValueOfEnabledNumber( out int originalValue );
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNumberIsActive( DifferentEnabledNumber );
        AssertNumberIsSelected( DifferentEnabledNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DifferentEnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickEnabledNumberWithSameValue_RemoveCandidate_DeselectsNumber_KeepsCellSelected_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellDoesNotHaveSpecificCandidate( affectedCell, EnabledNumber );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickEnabledNumberWithDifferentValue_AddCandidate_DeselectsNumber_KeepsCellSelected_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertCellContainsCandidate( affectedCell, DifferentEnabledNumber );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickEnabledNumberWithSameValue_KeepsCellValue_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNumberIsActive( EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickEnabledNumberWithDifferentValue_KeepsCellValue_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNumberIsActive( DifferentEnabledNumber );
        AssertNumberIsSelected( DifferentEnabledNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DifferentEnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickEnabledNumberWithSameValue_KeepsCellValue_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNumberIsActive( EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickEnabledNumberWithDifferentValue_KeepsCellValue_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertNumberIsActive( DifferentEnabledNumber );
        AssertNumberIsSelected( DifferentEnabledNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DifferentEnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEraser_ClickEnabledNumber_DeselectsEraser_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEraser();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertEraserIsNotSelected();
        AssertNumberIsActive( EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickEnabledNumber_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNumberIsActive( EnabledNumber );
        AssertNumberIsNotActive( DisabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickEnabledNumber_DeselectsNumber_UnhighlightsEverything()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickDifferentEnabledNumber_KeepsPencilSelected_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        SelectDifferentEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsActive( DifferentEnabledNumber );
        AssertNumberIsSelected( DifferentEnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentEnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DifferentEnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickEnabledNumber_KeepsPencilSelected_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsActive( EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickEnabledNumber_KeepsPencilSelected_DeselectsNumber_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        SelectEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertNoCellIsHighlighted();
    }
}
