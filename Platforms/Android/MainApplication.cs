using Android.App;
using Android.Runtime;
using YASudoku.Common;

namespace YASudoku;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
        AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
    }

    private void AndroidEnvironment_UnhandledExceptionRaiser( object? sender, RaiseThrowableEventArgs e )
    {
        ExceptionLogging.LogException( e );
        e.Handled = true;
        Environment.Exit( 0 );
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
