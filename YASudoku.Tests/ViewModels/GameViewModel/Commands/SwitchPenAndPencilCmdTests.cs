using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class SwitchPenAndPencilCmdTests : GameVMTestsBase
{
    [Fact]
    public void WithNoActiveNumberOrCell_ClickPencil_SelectsPencil_NoCellIsSelectedOrHighlighted()
    {
        //Arrange
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActiveEnabledNumber_ClickPencil_SelectsPencil_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        ActivateEnabledNumber();
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActiveDisabledNumber_ClickPencil_SelectsPencil_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        ActivateDisabledNumber();
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertNoCellIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActiveEmptyCell_ClickPencil_SelectsPencil_CellHasNoValue_CellIsSelected_CellAndRelatedCellsAreHighlighted()
    {
        //Arrange
        GameGridCellVisualData affectedCell = ActivateEmptyCell( out _ );
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellHasNoValue( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveLockedCell_ClickPencil_SelectsPencil_KeepsCellValue_CellIsSelected_CellAndRelatedCellsAreHighlighted_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        GameGridCellVisualData affectedCell = ActivateLockedCell( out int originalValue );
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCandidates_ClickPencil_SelectsPencil_CellIsSelected_CellAndRelatedCellsAreHighlighted()
    {
        //Arrange
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithCorrectValue_ClickPencil_SelectsPencil_CellIsSelected_CellAndRelatedCellsAreHighlighted_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveCellWithIncorrectValue_ClickPencil_SelectsPencil_CellIsSelected_CellAndRelatedCellsAreHighlighted_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActiveEraser_ClickPencil_SelectsPencil_DeselectsEraser_NoCellIsSelectedOrHighlighted()
    {
        //Arrange
        ActivateEraser();
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsSelected();
        AssertEraserIsNotSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencil_ClickPencil_DeselectsPencil_NoCellIsSelectedOrHighlighted()
    {
        //Arrange
        ActivatePencil();
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsNotSelected();
        AssertNoCellIsSelected();
        AssertNoCellIsHighlighted();
    }

    [Fact]
    public void WithActivePencilAndWithActiveEnabledNumber_ClickPencil_DeselectsPencil_KeepsSelectedNumber_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        ActivatePencil();
        ActivateEnabledNumber();
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsNotSelected();
        AssertEnabledNumberIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( EnabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( EnabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveDisabledNumber_ClickPencil_DeselectsPencil_KeepsSelectedNumber_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        ActivatePencil();
        ActivateDisabledNumber();
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsNotSelected();
        AssertDisabledNumberIsSelected();
        AssertCellsWithValueAreHighlightedAsSelected( DisabledNumber );
        AssertCellsWithDifferentValueAreNotHighlighted( DisabledNumber );
    }

    [Fact]
    public void WithActivePencilAndWithActiveEmptyCell_ClickPencil_DeselectsPencil_CellHasNoValue_CellIsSelected_CellAndRelatedCellsAreHighlighted()
    {
        //Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateEmptyCell( out _ );
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellHasNoValue( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveLockedCell_ClickPencil_DeselectsPencil_KeepsCellValue_CellIsSelected_CellAndRelatedCellsAreHighlighted_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateLockedCell( out int originalValue );
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCandidates_ClickPencil_DeselectsPencil_CellIsSelected_CellAndRelatedCellsAreHighlighted()
    {
        //Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellWithCandidateOfEnabledNumber();
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertNoOtherAndUnrelatedCellsAreHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithCorrectValue_ClickPencil_DeselectsPencil_KeepsCellValue_CellIsSelected_CellAndRelatedCellsAreHighlighted_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithCorrectValueOfEnabledNumber( out int originalValue );
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }

    [Fact]
    public void WithActivePencilAndWithActiveCellWithIncorrectValue_ClickPencil_DeselectsPencil_KeepsCellValue_CellIsSelected_CellAndRelatedCellsAreHighlighted_CellsWithSameValueAreHighlighted()
    {
        //Arrange
        ActivatePencil();
        GameGridCellVisualData affectedCell = ActivateCellFilledWithIncorrectValueOfEnabledNumber( out int originalValue );
        //Act
        ActivatePencil();
        //Assert
        AssertPencilIsNotSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsFilledWithSpecificValue( affectedCell, originalValue );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
        AssertCellsWithValueAreHighlightedAsSelected( originalValue );
        AssertUnrelatedCellsWithDifferentValueAreNotHighlighted( affectedCell );
    }
}
