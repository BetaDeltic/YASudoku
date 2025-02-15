using CommunityToolkit.Mvvm.ComponentModel;

namespace YASudoku.Controls.ControlBindings;

public partial class LabelOrNumberedGridBinding : ObservableObject
{
    [ObservableProperty]
    public partial string Text { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsVisible { get; set; } = true;

    [ObservableProperty]
    public partial Color BackgroundColor { get; set; } = Colors.Transparent;

    [ObservableProperty]
    public partial Color TextColor { get; set; } = Colors.Transparent;
}
