using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class PauseGameCmdTests : GameVMTestsBase
{
    private void PausedCommonAssertions()
    {
        AssertAllCellsAreHidingValues();
        AssertNoCellIsHighlighted();
        AssertIsPaused();
    }

    private void UnpausedCommonAssertions()
    {
        AssertNoCellIsHidingValues();
        AssertIsNotPaused();
    }

    [Fact]
    public void WithNoActiveNumberOrCell_ClickPause_NoNumberIsActive_NoCellIsActive()
    {
        // Arrange
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertNoNumberIsActive();
        AssertNoNumberIsSelected();
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickPause_SelectedNumberStaysActive()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertEnabledNumberIsActive();
        AssertEnabledNumberIsSelected();
        AssertNoCellIsSelected();
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickPause_SelectedNumberStaysActive()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertDisabledNumberIsActive();
        AssertDisabledNumberIsSelected();
        AssertNoCellIsSelected();
    }

    [Fact]
    public void WithActiveEmptyCell_ClickPause_SelectedCellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateEmptyCell( out _ );
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActiveLockedCell_ClickPause_SelectedCellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateLockedCell( out _ );
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickPause_SelectedCellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickPause_SelectedCellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickPause_SelectedCellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActiveEraser_ClickPause_EraserStaysActive()
    {
        // Arrange
        ActivateEraser();
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertEraserIsSelected();
    }

    [Fact]
    public void WithActivePencil_ClickPause_PencilStaysActive()
    {
        // Arrange
        ActivatePencil();
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertPencilIsSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickPause_PencilStaysActive_SelectedNumberStaysActive()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertPencilIsSelected();
        AssertEnabledNumberIsActive();
        AssertEnabledNumberIsSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickPause_PencilStaysActive_SelectedNumberStaysActive()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertPencilIsSelected();
        AssertDisabledNumberIsSelected();
        AssertDisabledNumberIsActive();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickPause_PencilStaysActive_SelectedCellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateEmptyCell( out _ );
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickPause_PencilStaysActive_SelectedCellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateLockedCell( out _ );
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_PencilStaysActive_SelectedCellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_PencilStaysActive_SelectedCellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_PencilStaysActive_SelectedCellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        TogglePause();
        // Assert
        PausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
    }

    [Fact]
    public void WithNoActiveNumberOrCell_ClickPause_ClickUnpause_NoNumberIsActive_NoCellIsActive()
    {
        // Arrange
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertNoNumberIsActive();
        AssertNoCellIsSelected();
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickPause_ClickUnpause_NumberStaysActive_CellsWithSameValueAreHighlighted()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertEnabledNumberIsActive();
        AssertEnabledNumberIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickPause_ClickUnpause_NumberStaysActive_CellsWithSameValueAreHighlighted()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertDisabledNumberIsActive();
        AssertDisabledNumberIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickPause_ClickUnpause_CellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateEmptyCell( out _ );
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( selectedCell );
    }

    [Fact]
    public void WithActiveLockedCell_ClickPause_ClickUnpause_CellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateLockedCell( out int originalValue );
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( selectedCell );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickPause_ClickUnpause_CellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( selectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickPause_ClickUnpause_CellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( selectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickPause_ClickUnpause_CellStaysActive()
    {
        // Arrange
        GameGridCellVisualData selectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( selectedCell );
    }
    
    [Fact]
    public void WithActiveEraser_ClickPause_ClickUnpause_EraserStaysActive()
    {
        // Arrange
        ActivateEraser();
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertEraserIsSelected();
    }

    [Fact]
    public void WithActivePencil_ClickPause_ClickUnpause_PencilStaysActive()
    {
        // Arrange
        ActivatePencil();
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertPencilIsSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickPause_ClickUnpause_PencilStaysActive_NumberStaysActive_CellsWithSameValueAreHighlighted()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertPencilIsSelected();
        AssertEnabledNumberIsActive();
        AssertEnabledNumberIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickPause_ClickUnpause_PencilStaysActive_NumberStaysActive_CellsWithSameValueAreHighlighted()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertPencilIsSelected();
        AssertDisabledNumberIsActive();
        AssertDisabledNumberIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickPause_ClickUnpause_PencilStaysActive_CellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateEmptyCell( out _ );
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( selectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickPause_ClickUnpause_PencilStaysActive_CellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateLockedCell( out int originalValue );
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( selectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickPause_ClickUnpause_PencilStaysActive_CellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( selectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickPause_ClickUnpause_PencilStaysActive_CellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( selectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickPause_ClickUnpause_PencilStaysActive_CellStaysActive()
    {
        // Arrange
        ActivatePencil();
        GameGridCellVisualData selectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        // Act
        TogglePause();
        TogglePause();
        // Assert
        UnpausedCommonAssertions();
        AssertPencilIsSelected();
        AssertCellIsSelected( selectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( selectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( selectedCell );
    }
}
