using YASudoku.Models;

namespace YASudoku.Tests.ViewModels.GameViewModel;

public class GameVMTests : GameVMTestsBase
{
    [Fact]
    public void UpdateNumberCount_ToZero_DisablesGivenNumber_KeepsItActiveForRemovals()
    {
        if ( gameVM.VisualState?.GameData == null ) throw new SystemException( gameDataNotInitialized );
        // Arrange
        gameVM.VisualState!.NumPadVS.DeselectCurrentNumber();
        const int affectedNumber = 1;
        GameDataContainer newGameData = TestsCommon.CreateGameDataWithSpecificSequence( Enumerable.Repeat( affectedNumber, 9 ) );
        gameVM.VisualState.GameData.ReplaceCollection( newGameData.AllCells );
        gameVM.PressNumber( affectedNumber );

        // Act
        gameVM.VisualState.UpdateButtonRemainingCount( affectedNumber );

        // Assert
        AssertButtonIsDisabled( affectedNumber );
        AssertButtonIsActive( affectedNumber );
        AssertButtonRemainingCountIsExpected( affectedNumber, 0 );
    }
}
