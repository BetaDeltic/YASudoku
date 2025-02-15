namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class NewGameCmdTests : GameCommandsTestsBase
{
    public NewGameCmdTests()
    {
        IndexOfTestedCell = GetUnusedIndexOfFilledCellAndSaveIt();
        originalValue = ChangeTestedCellValue();
    }

    private async Task ClickNewGame()
    {
        SendCompletionSignalsOnAnimations();
        await gameVM.StartNewGame();
    }

    [Theory]
    [MemberData( nameof( ArrangeActions ) )]
    public async Task UnderAllCircumstances_ClickRestart_GameDataCollectionIsChanged_NoNumberIsActive_NoCellIsActive_NoButtonIsActive( Action arrangeAction )
    {
        // Arrange
        arrangeAction();
        // Act
        await ClickNewGame();
        // Assert
        CommonAssertions();
    }
}
