namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class RestartGameCmdTests : GameCommandsTestsBase
{
    public RestartGameCmdTests()
    {
        IndexOfTestedCell = GetUnusedIndexOfEmptyCellAndSaveIt();
        originalValue = ChangeTestedCellValue();
    }

    private async Task ClickRestartGame()
    {
        SendCompletionSignalsOnAnimations();
        await gameVM.RestartGame();
    }

    [Theory]
    [MemberData( nameof( ArrangeActions ) )]
    public async Task UnderAllCircumstances_ClickRestart_GameDataCollectionIsChanged_NoNumberIsActive_NoCellIsActive_NoButtonIsActive( Action arrangeAction )
    {
        // Arrange
        arrangeAction();
        // Act
        await ClickRestartGame();
        // Assert
        CommonAssertions();
    }
}
