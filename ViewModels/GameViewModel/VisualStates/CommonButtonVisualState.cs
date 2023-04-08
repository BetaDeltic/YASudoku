using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class CommonButtonVisualState : ObservableObject
{
    [ObservableProperty]
    public Color backgroundColor = Colors.Transparent;

    [ObservableProperty]
    public Color textColor = Colors.Transparent;

    [ObservableProperty]
    public bool isActive;

    private readonly Color foregroundColor;
    private readonly Color accentColor;

    public CommonButtonVisualState( ISettingsService settings )
    {
        foregroundColor = SettingsService.ForegroundColor;
        accentColor = settings.GetAccentColor();

        TextColor = foregroundColor;
        BackgroundColor = accentColor;
    }

    public void DeactivateButton()
    {
        if ( !IsActive ) return;

        IsActive = false;
        TextColor = foregroundColor;
        BackgroundColor = accentColor;
    }

    public void ActivateButton()
    {
        if ( IsActive ) return;

        IsActive = true;
        TextColor = accentColor;
        BackgroundColor = foregroundColor;
    }
}
