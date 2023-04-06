using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels;

public partial class VMsBase : ObservableObject
{
    [ObservableProperty]
    private Color _accentColor = Colors.DarkMagenta;

    [ObservableProperty]
    private Color _foregroundColor = Colors.White;

    private readonly ISettingsService settings;

    public VMsBase( ISettingsService settingsService )
    {
        settings = settingsService;

        AccentColor = settings.GetAccentColor();
        ForegroundColor = SettingsService.ForegroundColor;
    }
}
