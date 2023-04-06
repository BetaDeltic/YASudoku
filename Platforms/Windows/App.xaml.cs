﻿using YASudoku.Common;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YASudoku.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		InitializeComponent();

        Current.UnhandledException += Current_UnhandledException;
    }

    private static void Current_UnhandledException( object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e )
	{
		ExceptionLogging.LogException( e );
		e.Handled = true;
		Environment.Exit( 0 );
	}

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
