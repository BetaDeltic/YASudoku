using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class SelectCellCmdTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumber_ClickEmptyCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell();
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
        GameGridCellVisualData affectedCell = ClickEmptyCell();
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
        ActivateEmptyCell();
        // Act
        GameGridCellVisualData affectedCell = ClickEmptyCell();
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
        GameGridCellVisualData affectedCell = ClickEmptyCell();
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
        ActivateEmptyCell();
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
        GameGridCellVisualData affectedCell = ClickEmptyCell();
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
        ClickEmptyCell();
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
        GameGridCellVisualData affectedCell = ClickEmptyCell();
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
        GameGridCellVisualData affectedCell = ClickEmptyCell();
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
        GameGridCellVisualData affectedCell = ClickEmptyCell();
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
    public void WithActivePencilAndWithActiveEraser_ClickEmptyCell_NoCellIsSelected_NoCellIsHighlighted_KeepsPencilActive_KeepsEraserActive()
    {
        // Arrange
        ActivatePencil();
        ActivateEraser();
        // Act
        ClickEmptyCell();
        // Assert
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
        AssertPencilIsSelected();
        AssertEraserIsSelected();
    }

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
    public void
        WithActivePencil_ClickLockedCell_SelectsCell_KeepsCellValue_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue_KeepsPencilActive()
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
    public void WithActiveEnabledNumber_ClickCellWithSameNumberAsCandidate_FillsCellWithValue_KeepsNumberSelected_DeselectsCell_HighlightsAllCellsWithSameNumber()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsFilledWithSpecificValue( affectedCell, EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickCellWithDifferentNumberAsCandidate_FillsCellWithValue_KeepsNumberSelected_DeselectsCell_HighlightsAllCellsWithSameNumber()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfDifferentEnabledNumber();
        // Assert
        AssertCellIsFilledWithSpecificValue( affectedCell, EnabledNumber );
        AssertNumberIsSelected( EnabledNumber );
        AssertNoCellIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickCellWithSameNumberAsCandidate_DeselectsNumber_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfDisabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void
        WithActiveDisabledNumber_ClickCellWithCandidateOfEnabledNumber_DeselectsNumber_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertNoNumberIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithNoActiveCell_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEraser_ClickCellWithCandidates_RemovesAllCandidates_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateEraser();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
        AssertCellHasNoCandidates( affectedCell );
    }

    [Fact]
    public void WithActivePencil_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickCellWithSameNumberAsCandidate_RemovesCandidate_KeepsNumberSelected_HighlightsSameValues()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( EnabledNumber );
        AssertCellDoesNotHaveSpecificCandidate( affectedCell, EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickCellWithoutSameNumberAsCandidate_AddsCandidate_KeepsNumberSelected_HighlightsSameValues()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfDifferentEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( EnabledNumber );
        AssertCellContainsCandidate( affectedCell, EnabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickCellWithSameNumberAsCandidate_RemovesCandidate_KeepsNumberSelected_HighlightsSameValues()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfDisabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertCellDoesNotHaveSpecificCandidate( affectedCell, DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickCellWithoutSameNumberAsCandidate_AddsCandidate_KeepsNumberSelected_HighlightsSameValues()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfDifferentEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNumberIsSelected( DisabledNumber );
        AssertCellContainsCandidate( affectedCell, DisabledNumber );
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEraser_ClickCellWithCandidates_RemovesAllCandidates_KeepsPencilSelected_KeepsEraserSelected_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateEraser();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertEraserIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
        AssertCellHasNoCandidates( affectedCell );
    }

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
    public void WithActiveCellWithCandidates_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
        // Act
        ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
        // Act
        ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateEmptyCell();
        // Act
        ClickEmptyCell();
        // Assert
        AssertPencilIsSelected();
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
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
        // Act
        ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickSameCell_DeselectsCell_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
        // Act
        ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveEmptyCell_ClickDifferentEmptyCell_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateEmptyCell();
        // Act
        GameGridCellVisualData affectedCell = ClickDifferentEmptyCell();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
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

    [Fact]
    public void
        WithActiveCellWithCandidates_ClickDifferentCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( affectedCell.UserFacingValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateEmptyCell();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveLockedCell_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateLockedCell();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        ActivateEmptyCell();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        ActivateLockedCell();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickCellWithCandidates_DeselectsCell_KeepsPencilActive_UnhighlightsEverything()
    {
        // Arrange
        ActivatePencil();
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivatePencil();
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickDifferentCellWithCorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
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
        ActivateEmptyCell();
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
        ActivateLockedCell();
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
        ActivateEmptyCell();
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
        ActivateLockedCell();
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
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
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
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
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
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
        // Act
        ClickCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickDifferentCellWithIncorrectValue_SelectsCell_HighlightsCellAndRelatedCells_HighlightsCellsWithSameValue()
    {
        // Arrange
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
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
        ActivateEmptyCell();
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
        ActivateLockedCell();
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
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
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
        ActivateEmptyCell();
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
        ActivateLockedCell();
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
        ActivateCellFilledWithCorrectValueOfEnabledNumber();
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
        ActivateCellFilledWithIncorrectValueOfEnabledNumber();
        // Act
        ClickCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

}
