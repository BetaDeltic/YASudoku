using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class SelectEraserCmdTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumberOrCell_ClickEraser_EraserIsActive()
    {
        // Arrange
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsSelected();
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickEraser_EraserIsActive_DeselectsNumber_NoCellIsHighlightedOrSelected()
    {
        // Arrange
        ActivateEnabledNumber();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsSelected();
        AssertNumberIsNotActive( EnabledNumber );
        AssertNoCellIsHighlighted();
        AssertNoCellIsSelected();
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickEraser_EraserIsActive_DeselectsNumber_NoCellIsHighlightedOrSelected()
    {
        // Arrange
        ActivateDisabledNumber();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsSelected();
        AssertNumberIsNotActive( DisabledNumber );
        AssertNoCellIsHighlighted();
        AssertNoCellIsSelected();
    }

    [Fact]
    public void WithActiveEmptyCell_ClickEraser_EraserIsDeselected_CellAndRelatedCellsAreHighlightedAndSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateEmptyCell( out _ );
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveLockedCell_ClickEraser_EraserIsDeselected_CellIsDeselected_NoCellIsHighlighted()
    {
        // Arrange
        ActivateLockedCell( out _ );
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickEraser_CellHasNoCandidates_EraserIsDeselected_CellRemainsSelected_CellAndRelatedCellsAreHighlighted()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickEraser_CellHasNoValue_EraserIsDeselected_CellRemainsSelected_CellAndRelatedCellsAreHighlighted()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickEraser_CellHasNoValue_EraserIsDeselected_CellRemainsSelected_CellAndRelatedCellsAreHighlighted()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEraser_ClickEraser_EraserIsDeselected()
    {
        // Arrange
        ActivateEraser();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
    }

    [Fact]
    public void WithActivePencil_ClickEraser_EraserIsActive_PencilIsDeselected()
    {
        // Arrange
        ActivatePencil();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsSelected();
        AssertPencilIsNotSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickEraser_EraserIsActive_PencilIsDeselected_NumberIsDeselected_NoCellIsHighlightedOrSelected()
    {
        // Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsSelected();
        AssertPencilIsNotSelected();
        AssertNumberIsNotActive( EnabledNumber );
        AssertNoCellIsHighlighted();
        AssertNoCellIsSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickEraser_EraserIsActive_PencilIsDeselected_NumberIsDeselected_NoCellIsHighlightedOrSelected()
    {
        // Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsSelected();
        AssertPencilIsNotSelected();
        AssertNumberIsNotActive( DisabledNumber );
        AssertNoCellIsHighlighted();
        AssertNoCellIsSelected();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickEraser_PencilIsDeselected_EraserIsDeselected_CellAndRelatedCellsAreHighlightedAndSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateEmptyCell( out _ );
        ActivatePencil();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertPencilIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickEraser_PencilIsDeselected_EraserIsDeselected_CellIsDeselected_NoCellIsHighlighted()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateLockedCell( out _ );
        ActivatePencil();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertPencilIsNotSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickEraser_PencilIsDeselected_EraserIsDeselected_CellValueIsErased_CellAndRelatedCellsAreHighlightedAndSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        ActivatePencil();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertPencilIsNotSelected();
        AssertCellHasNoValue( affectedCell );
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickEraser_PencilIsDeselected_EraserIsDeselected_CellValueIsErased_CellAndRelatedCellsAreHighlightedAndSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out _ );
        ActivatePencil();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertPencilIsNotSelected();
        AssertCellHasNoValue( affectedCell );
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickEraser_PencilIsDeselected_EraserIsDeselected_CellValueIsErased_CellAndRelatedCellsAreHighlightedAndSelected()
    {
        // Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out _ );
        ActivatePencil();
        // Act
        ActivateEraser();
        // Assert
        AssertEraserIsNotSelected();
        AssertPencilIsNotSelected();
        AssertCellHasNoValue( affectedCell );
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }
}
