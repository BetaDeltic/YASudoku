using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using YASudoku.Services.ResourcesService;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels;

public partial class SettingsFlyoutVM : VMsBase
{
    [ObservableProperty]
    private bool _isFlyoutVisibleInternal;

    [ObservableProperty]
    private bool _isHighlightRelatedEnabled = true;

    [ObservableProperty]
    private bool _isHighlightMistakesEnabled = true;

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
