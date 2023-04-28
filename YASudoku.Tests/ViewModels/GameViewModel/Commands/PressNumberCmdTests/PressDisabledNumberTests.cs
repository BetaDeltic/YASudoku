using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands.PressNumberCmdTests;

public class PressDisabledNumberTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumber_ClickDisabledNumber_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNumberIsActive( DisabledNumber );
        AssertNumberIsSelected( DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithNoActiveCell_ClickDisabledNumber_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNumberIsActive( DisabledNumber );
        AssertNumberIsSelected( DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickDisabledNumber_DeselectsNumber_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateEmptyCell( out _ );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveLockedCell_ClickDisabledNumberWithSameValue_KeepsCellValue_ActivatesNumber_DeselectsCell_HighlightsCellsWithSameValue()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateLockedCellWithValueOfDisabledNumber( out int originalValue );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsDisabled( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveLockedCell_ClickDisabledNumberWithDifferentValue_KeepsCellValue_ActivatesNumber_DeselectsCell_HighlightsCellsWithSameValue()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateLockedCellWithValueOfEnabledNumber( out int originalValue );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsDisabled( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickDisabledNumberWithSameValue_DeselectsNumber_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellWithCandidateOfDisabledNumber();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickDisabledNumberWithDifferentValue_DeselectsNumber_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickDisabledNumberWithSameValue_RemovesCellValue_KeepsCellSelected_DeselectsNumber_EnablesNumber_HighlightsCellAndRelatedCells()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfDisabledNumber( out _ );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNumberIsEnabled( DisabledNumber );
        AssertNumberIsNotActive( DisabledNumber );
        AssertCellHasNoValue( affectedCell );
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickDisabledNumberWithDifferentValue_DeselectsNumber_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickDisabledNumberWithSameValue_RemovesCellValue_KeepsCellSelected_DeselectsNumber_EnablesNumber_HighlightsCellAndRelatedCells()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfDisabledNumber( out _ );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNumberIsEnabled( DisabledNumber );
        AssertNumberIsNotActive( DisabledNumber );
        AssertCellHasNoValue( affectedCell );
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickDisabledNumberWithDifferentValue_DeselectsNumber_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNoNumberIsActive();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveEraser_ClickDisabledNumber_DeselectsEraser_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateEraser();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertEraserIsNotSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencil_ClickDisabledNumber_KeepsPencilSelected_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickDisabledNumber_AddsCandidate_DeselectsNumber_KeepsCellSelected_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateEmptyCell( out _ );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNumberIsDisabled( DisabledNumber );
        AssertNumberIsNotActive( DisabledNumber );
        AssertCellContainsCandidate( affectedCell, DisabledNumber );
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickDisabledNumberWithSameValue_KeepsCellValue_KeepsPencilSelected_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateLockedCellWithValueOfDisabledNumber( out int originalValue );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickDisabledNumberWithDifferentValue_KeepsCellValue_KeepsPencilSelected_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateLockedCell( out int originalValue );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickDisabledNumberWithSameValue_RemovesCandidate_KeepsPencilSelected_KeepsCellSelected_DeselectsNumber_HighlightsCellsAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfDisabledNumber();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNumberIsDisabled( DisabledNumber );
        AssertNumberIsNotActive( DisabledNumber );
        AssertCellDoesNotHaveSpecificCandidate( affectedCell, DisabledNumber );
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickDisabledNumberWithDifferentValue_KeepsCandidates_KeepsPencilSelected_KeepsCellSelected_DeselectsNumber_HighlightsCellsAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNumberIsDisabled( DisabledNumber );
        AssertNumberIsNotActive( DisabledNumber );
        AssertCellContainsCandidate( affectedCell, EnabledNumber );
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickDisabledNumberWithSameValue_KeepsCellValue_KeepsPencilSelected_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfDisabledNumber( out int originalValue );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickDisabledNumberWithDifferentValue_KeepsCellValue_KeepsPencilSelected_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickDisabledNumberWithSameValue_KeepsCellValue_KeepsPencilSelected_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfDisabledNumber( out int originalValue );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickDisabledNumberWithDifferentValue_KeepsCellValue_KeepsPencilSelected_DeselectsCell_KeepsNumberSelected_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickDisabledNumber_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickDisabledNumber_DeselectsNumber_UnhighlightsEverything()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertNumberIsDisabled( DisabledNumber );
        AssertNumberIsNotActive( DisabledNumber );
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickDifferentDisabledNumber_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        SelectDifferentDisabledNumber();
        // Assert
        AssertNumberIsSelected( DifferentDisabledNumber );
        AssertNumberIsActive( DifferentDisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentDisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DifferentDisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickDisabledNumber_KeepsPencilSelected_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertNumberIsActive( DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickDifferentDisabledNumber_KeepsPencilSelected_SelectsNumber_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        SelectDifferentDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DifferentDisabledNumber );
        AssertNumberIsActive( DifferentDisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DifferentDisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DifferentDisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickDisabledNumber_KeepsPencilSelected_DeselectsNumber_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        SelectDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNoNumberIsSelected();
        AssertNumberIsDisabled( DisabledNumber );
        AssertNumberIsNotActive( DisabledNumber );
        AssertNoCellIsHighlighted();
    }

}
