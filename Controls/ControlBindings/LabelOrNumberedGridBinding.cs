using CommunityToolkit.Mvvm.ComponentModel;

namespace YASudoku.Controls.ControlBindings;

public partial class LabelOrNumberedGridBinding : ObservableObject
{
    [ObservableProperty]
    private string text = string.Empty;

    [ObservableProperty]
    private bool isVisible = true;

    [ObservableProperty]
    private Color backgroundColor = Colors.Transparent;

    [ObservableProperty]
    private Color textColor = Colors.Transparent;
}
