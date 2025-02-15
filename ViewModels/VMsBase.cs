using CommunityToolkit.Mvvm.ComponentModel;
using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels;

public partial class VMsBase : ObservableObject
{
    [ObservableProperty]
    public partial Color PrimaryColor { get; set; }

    [ObservableProperty]
    public partial Color SecondaryColor { get; set; }

    private readonly ISettingsService settings;

    public VMsBase( ISettingsService settingsService, IResourcesService resourcesService )
    {
        settings = settingsService;
        PrimaryColor = settings.GetPrimaryColor();

        resourcesService.TryGetColorByName( "SecondaryColor", out Color secondaryColor );
        SecondaryColor = secondaryColor;

        if ( PrimaryColor == null || SecondaryColor == null )
            throw new NullReferenceException( "Unable to initialize default colors." );
    }
}
