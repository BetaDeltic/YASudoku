using YASudoku.Models;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class PressNumberCmdTests : GameVMTestsBase
{
    [Fact]
    public void WithoutActiveNumber_ActivatesNumber_HighlightsOnlyCellsWithSameNumber()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );
        // Arrange
        gameVM.VisualState.NumPadVS.DeselectCurrentNumber();
        GameDataContainer newGameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Range( 1, 2 ) );
        gameVM.VisualState.GameData.ReplaceCollection( newGameData.AllCells );

        // Act
        const int pressedNumber = 1;
        gameVM.PressNumber( pressedNumber );

        // Assert
        AssertButtonIsPressed( pressedNumber );
        AssertButtonIsEnabled( pressedNumber );
        AssertSameCellsAreHighlightedAsSelected( pressedNumber );
        AssertDifferentCellsAreNotHighlighted( pressedNumber );
    }

    [Fact]
    public void WithActiveNumber_DeactivatesPreviousOneActivatesNewOne_HighlightsOnlyCellWithSameNumber()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );
        // Arrange
        gameVM.VisualState.NumPadVS.SelectedButtonNumber = 1;
        GameDataContainer newGameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Range( 1, 2 ) );
        gameVM.VisualState.GameData.ReplaceCollection( newGameData.AllCells );

        const int pressedNumber = 2;
        GetNumPadButtonFromNumber( pressedNumber ).UpdateRemainingCount( 9 );

        // Act
        gameVM.PressNumber( pressedNumber );

        // Assert
        AssertButtonIsPressed( pressedNumber );
        AssertButtonIsEnabled( pressedNumber );
        AssertSameCellsAreHighlightedAsSelected( pressedNumber );
        AssertDifferentCellsAreNotHighlighted( pressedNumber );
    }

    [Fact]
    public void OnDisabledNumber_HavingSameNumberSelectedOnGrid_RemovesTheNumberFromGrid_KeepsCellSelected_ReEnablesTheNumber()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );
        // Arrange
        gameVM.VisualState.NumPadVS.DeselectCurrentNumber();
        const int affectedNumber = 1;
        GameDataContainer newGameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Repeat( affectedNumber, 9 ) );
        gameVM.VisualState.GameData.ReplaceCollection( newGameData.AllCells );
        gameVM.VisualState.UpdateButtonRemainingCount( 1 );
        gameVM.VisualState.GameGridVS.SelectNewCell( 0 );

        // Act
        GameGridCellVisualData? affectedCell = gameVM.VisualState.SelectedCell;
        gameVM.PressNumber( affectedNumber );

        // Assert
        AssertNoButtonIsSelected();
        AssertCellIsSelected( affectedCell );
        AssertCellIsEmpty( affectedCell );
        AssertCellIsHighlightedAsSelected( affectedCell );
    }
}
