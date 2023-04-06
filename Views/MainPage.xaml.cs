using YASudoku.ViewModels;

namespace YASudoku.Views;

public partial class MainPage : ContentPage
{
    public MainPage( MainVM mainVM )
    {
        InitializeComponent();
        BindingContext = mainVM;

        GameBtn.CommandParameter = true;

        GameBtn.SetBinding( BackgroundColorProperty, nameof( MainVM.AccentColor ) );
        GameBtn.SetBinding( Button.TextColorProperty, nameof( MainVM.ForegroundColor ) );
    }
}
