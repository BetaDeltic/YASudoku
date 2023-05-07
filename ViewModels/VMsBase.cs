using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels;

public partial class VMsBase : ObservableObject
{
    [ObservableProperty]
    private Color _primaryColor;

    [ObservableProperty]
    private Color _secondaryColor;

    private readonly ISettingsService settings;

    public VMsBase( ISettingsService settingsService, IResourcesService resourcesService )
    {
        settings = settingsService;
        PrimaryColor = settings.GetPrimaryColor();

        resourcesService.TryGetColorByName( "SecondaryColor", out Color secondaryColor );
        SecondaryColor = secondaryColor;

        if ( _primaryColor == null || _secondaryColor == null )
            throw new NullReferenceException( "Unable to initialize default colors." );
    }
}
