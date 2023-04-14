using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class SettingsCmd
{
    private readonly SettingsVisualState settingsVS;

    public SettingsCmd( SettingsVisualState settingsVS )
    {
        this.settingsVS = settingsVS;
    }

    public void OpenSettings() => settingsVS.AreSettingsVisible = true;
}
