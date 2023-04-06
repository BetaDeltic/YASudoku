using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public class CommonButtonVisualState : ObservableObject
{
    public Color BackgroundColor
    {
        get => _backgroundColor;
        private set {
            _backgroundColor = value;
            OnPropertyChanged( nameof( BackgroundColor ) );
        }
    }

    public Color TextColor
    {
        get => _color;
        private set {
            _color = value;
            OnPropertyChanged( nameof( TextColor ) );
        }
    }

    public bool IsActive
    {
        get => _isActive;
        private set {
            _isActive = value;
            OnPropertyChanged( nameof( IsActive ) );
        }
    }

    private bool _isActive = false;
    private Color _backgroundColor = Colors.Transparent;
    private Color _color = Colors.Transparent;

    private readonly Color foregroundColor;
    private readonly Color accentColor;

    public CommonButtonVisualState( ISettingsService settings )
    {
        foregroundColor = SettingsService.ForegroundColor;
        accentColor = settings.GetAccentColor();
        DeactivateButton();
    }

    public void DeactivateButton()
    {
        IsActive = false;
        TextColor = foregroundColor;
        BackgroundColor = accentColor;
    }

    public void ActivateButton()
    {
        IsActive = true;
        TextColor = accentColor;
        BackgroundColor = foregroundColor;
    }
}
