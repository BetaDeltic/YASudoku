using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands.SelectCellCmdTests;

public class SelectCellWithCandidateTests : GameVMTestsBase
{
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
    public void WithActiveCellWithCandidates_ClickDifferentCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateCellWithCandidateOfEnabledNumber();
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfDifferentEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickCellWithCandidates_SelectsCell_HighlightsCellAndRelatedCells()
    {
        // Arrange
        ActivateEmptyCell( out _ );
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
        ActivateLockedCell( out _ );
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
        ActivateEmptyCell( out _ );
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
        ActivateLockedCell( out _ );
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
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
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
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
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
        ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
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
        ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        GameGridCellVisualData affectedCell = ClickCellWithCandidateOfEnabledNumber();
        // Assert
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }
}
