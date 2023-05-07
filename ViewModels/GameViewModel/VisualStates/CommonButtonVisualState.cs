using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class CommonButtonVisualState : ObservableObject
{
    [ObservableProperty]
    public Color _backgroundColor;

    [ObservableProperty]
    public Color _textColor;

    [ObservableProperty]
    public bool _isActive;

    private readonly Color secondaryColor;
    private readonly Color primaryColor;

    public CommonButtonVisualState( ISettingsService settings, IResourcesService resources )
    {
        primaryColor = settings.GetPrimaryColor();
        resources.TryGetColorByName( "SecondaryColor", out secondaryColor );

        TextColor = secondaryColor;
        BackgroundColor = primaryColor;

        if ( _backgroundColor == null || _textColor == null )
            throw new NullReferenceException( "Unable to initialize default colors." );
    }

    public void DeactivateButton()
    {
        if ( !IsActive ) return;

        IsActive = false;
        TextColor = secondaryColor;
        BackgroundColor = primaryColor;
    }

    public void ActivateButton()
    {
        if ( IsActive ) return;

        IsActive = true;
        TextColor = primaryColor;
        BackgroundColor = secondaryColor;
    }
}
