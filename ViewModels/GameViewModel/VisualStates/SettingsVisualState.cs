using CommunityToolkit.Mvvm.ComponentModel;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class SettingsVisualState : ObservableObject
{
    [ObservableProperty]
    private bool _areSettingsVisible;
}
