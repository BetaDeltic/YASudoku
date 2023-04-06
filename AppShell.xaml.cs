using YASudoku.Views;

namespace YASudoku;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute( nameof( GamePage ), typeof( GamePage ) );
        Routing.RegisterRoute( nameof( LoadingPage ), typeof( LoadingPage ) );
    }
}
