namespace YASudoku.Tests.ViewModels.GameViewModel.Commands;

public class SettingsCmdTests : GameCommandsTestsBase
{
    private void ClickSettings() => gameVM.OpenSettings();

    private void AssertSettingsAreOpen() => Assert.True( VisualState.SettingsVS.AreSettingsVisible );

    [Theory]
    [MemberData( nameof( ArrangeActions ) )]
    public void UnderAllCircumstances_ClickSettings_OpensSettings( Action arrangeAction )
    {
        // Arrange
        arrangeAction();
        // Act
        ClickSettings();
        // Assert
        AssertSettingsAreOpen();
    }
}
