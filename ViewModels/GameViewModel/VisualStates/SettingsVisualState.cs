using CommunityToolkit.Mvvm.ComponentModel;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class SettingsVisualState : ObservableObject
{
    [ObservableProperty]
    public partial bool AreSettingsVisible { get; set; }
}
