using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels;

public partial class SettingsFlyoutVM : VMsBase
{
    [ObservableProperty]
    public partial bool IsFlyoutVisibleInternal { get; set; }

    [ObservableProperty]
    public partial bool IsHighlightRelatedEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsHighlightMistakesEnabled { get; set; } = true;

    private readonly ISettingsService settings;

    public SettingsFlyoutVM( IServiceProvider serviceProvider )
        : base( serviceProvider.GetService<ISettingsService>()!, serviceProvider.GetService<IResourcesService>()! )
    {
        settings = serviceProvider.GetService<ISettingsService>()!;

        IsHighlightRelatedEnabled = settings.CanHighlightRelatedCells();
        IsHighlightMistakesEnabled = settings.CanHighlightMistakes();
    }

    [RelayCommand]
    private void HideSettings() => IsFlyoutVisibleInternal = false;

    [RelayCommand]
    private void ToggleRelatedCellsSwitch( bool switchPosition )
        => settings.SetHighlightingRelatedCells( switchPosition );

    [RelayCommand]
    private void ToggleHighlightMistakesSwitch( bool switchPosition )
        => settings.SetHighlightingMistakes( switchPosition );
}
