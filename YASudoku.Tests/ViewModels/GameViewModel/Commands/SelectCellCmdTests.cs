using YASudoku.Models;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class SelectCellCmdTests : GameVMTestsBase
{
    [Fact]
    public void WithoutActiveNumber_WithSameCellActive_UnhighlightsCellAndRelatedCells()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );
        // Arrange
        gameVM.VisualState.NumPadVS.DeselectCurrentNumber();
        const int affectedCellIndex = 0;
        GameDataContainer newGameData = TestsCommon.CreateValidContainerWithEmptyCells();
        gameVM.VisualState.GameData.ReplaceCollection( newGameData.AllCells );
        gameVM.VisualState.GameGridVS.SelectNewCell( affectedCellIndex );

        // Act
        gameVM.SelectCell( affectedCellIndex );
        GameGridCellVisualData? affectedCell = gameVM.VisualState.GameData[ affectedCellIndex ];

        // Assert
        AssertNoButtonIsSelected();
        AssertNoCellIsSelected();
        AssertCellIsNotSelectedOrHighlighted( affectedCell );
        AssertRelatedCellsAreNotHighlighted( affectedCell );
    }

    // Test that verifies that cell is filled and cells with same number are highlighted when I select a number and then click on a cell.
    [Fact]
    public void WithActiveNumber_FillsCellWithNumber_HighlightsAllCellsWithSameNumber()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );
        // Arrange
        gameVM.VisualState.NumPadVS.DeselectCurrentNumber();
        const int affectedCellIndex = 0;
        const int affectedNumber = 1;
        GameDataContainer newGameData = TestsCommon.CreateValidContainerWithEmptyCells();
        gameVM.VisualState.GameData.ReplaceCollection( newGameData.AllCells );
        gameVM.PressNumber( affectedNumber );

        // Act
        gameVM.SelectCell( affectedCellIndex );

        // Assert
        AssertButtonIsPressed( affectedNumber );
        AssertButtonIsEnabled( affectedNumber );
        AssertNoCellIsSelected();
        AssertCellIsFilledWithNumber( affectedCellIndex, affectedNumber );
    }

    [Fact]
    public void WithoutActiveNumber_HighlightsCellAndRelatedCells()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );
        // Arrange
        gameVM.VisualState.NumPadVS.DeselectCurrentNumber();
        const int affectedCellIndex = 0;
        GameDataContainer newGameData = TestsCommon.CreateValidContainerWithEmptyCells();
        gameVM.VisualState.GameData.ReplaceCollection( newGameData.AllCells );

        // Act
        gameVM.SelectCell( affectedCellIndex );
        GameGridCellVisualData? affectedCell = gameVM.VisualState.SelectedCell;

        // Assert
        AssertNoButtonIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
    }

    [Fact]
    public void WithoutActiveNumber_WithDifferentCellActive_HighlightsCellAndRelatedCells()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );
        // Arrange
        gameVM.VisualState.NumPadVS.DeselectCurrentNumber();
        const int affectedCellIndex = 0;
        GameDataContainer newGameData = TestsCommon.CreateValidContainerWithEmptyCells();
        gameVM.VisualState.GameData.ReplaceCollection( newGameData.AllCells );
        gameVM.VisualState.GameGridVS.SelectNewCell( 1 );

        // Act
        gameVM.SelectCell( affectedCellIndex );
        GameGridCellVisualData? affectedCell = gameVM.VisualState.SelectedCell;

        // Assert
        AssertNoButtonIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
        AssertRelatedCellsAreHighlightedAsSelected( affectedCell );
    }
}
