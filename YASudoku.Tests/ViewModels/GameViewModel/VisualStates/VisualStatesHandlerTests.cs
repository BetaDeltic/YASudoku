using YASudoku.Models;

namespace YASudoku.Tests.ViewModels.GameViewModel.VisualStates;

public class VisualStatesHandlerTests : GameVMTestsBase
{
    [Fact]
    public void UpdateNumberCount_ToZero_DisablesGivenNumber_KeepsItActiveForRemovals()
    {
        // Arrange
        const int affectedNumber = 1;
        GameDataContainer newGameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Repeat( affectedNumber, 9 ) );
        GameData.ReplaceCollection( newGameData.AllCells );
        gameVM.PressNumber( affectedNumber );

        // Act
        VisualState.UpdateButtonRemainingCount( affectedNumber );

        // Assert
        AssertNumberIsDisabled( affectedNumber );
        AssertNumberIsActive( affectedNumber );
        AssertNumberRemainingCountIsExpected( affectedNumber, 0 );
    }
}
