using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Maui.LifecycleEvents;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Reflection;
using YASudoku.Models.PuzzleGenerators;

namespace YASudoku;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SetupSerilog();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureLifecycleEvents( events => {
#if WINDOWS
                events.AddWindows( windows => windows
                        .OnActivated( ( window, args ) => LogEvent( nameof( WindowsLifecycle.OnActivated ) ) )
                        .OnClosed( ( window, args ) => LogEvent( nameof( WindowsLifecycle.OnClosed ) ) )
                        .OnLaunched( ( window, args ) => LogEvent( nameof( WindowsLifecycle.OnLaunched ) ) )
                        .OnLaunching( ( window, args ) => LogEvent( nameof( WindowsLifecycle.OnLaunching ) ) )
                        .OnVisibilityChanged( ( window, args ) => LogEvent( nameof( WindowsLifecycle.OnVisibilityChanged ) ) )
                        .OnWindowCreated( window => window.ExtendsContentIntoTitleBar = false )
                );
#endif
#if ANDROID
                events.AddAndroid( android => android
                    .OnActivityResult( ( activity, requestCode, resultCode, data ) => LogEvent( nameof( AndroidLifecycle.OnActivityResult ), requestCode.ToString() ) )
                    .OnStart( ( activity ) => LogEvent( nameof( AndroidLifecycle.OnStart ) ) )
                    .OnCreate( ( activity, bundle ) => LogEvent( nameof( AndroidLifecycle.OnCreate ) ) )
                    .OnBackPressed( ( activity ) => LogEvent( nameof( AndroidLifecycle.OnBackPressed ) ) && false )
                    .OnStop( ( activity ) => LogEvent( nameof( AndroidLifecycle.OnStop ) ) )
                );
#endif
                static bool LogEvent( string eventName, string? type = null )
                {
                    Debug.WriteLine( $"Lifecycle event: {eventName}{( type == null ? string.Empty : $" ({type})" )}" );
                    return true;
                }
            } )
            .ConfigureFonts( fonts => {
                fonts.AddFont( "OpenSans-Regular.ttf", "OpenSansRegular" );
                fonts.AddFont( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
            } )
            .ConfigureContainer( new AutofacServiceProviderFactory(), containerBuilder => {
                Assembly assembly = Assembly.GetExecutingAssembly();

                containerBuilder.RegisterAssemblyTypes( assembly )
                    .Where( t => t.Name.EndsWith( "VM" ) || t.Name.EndsWith( "Page" ) || t.Name.EndsWith( "Flyout" ) )
                    .AsSelf()
                    .SingleInstance();

                containerBuilder.RegisterAssemblyTypes( assembly )
                    .Where( t => t.Name.EndsWith( "Service" ) || t.Name.EndsWith( "Handler" ) )
                    .AsImplementedInterfaces()
                    .SingleInstance();

                containerBuilder.RegisterType<TraditionalGenerator>().As<IPuzzleGenerator>();
            } );

        builder.Logging.AddSerilog( dispose: true );

        return builder.Build();
    }

    private static void SetupSerilog()
    {
        TimeSpan flushInterval = TimeSpan.FromSeconds( 1 );
        string file = Path.Combine( FileSystem.AppDataDirectory, "logs", "YASudoku.log" );

        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Warning()
        .MinimumLevel.Override( "Microsoft", LogEventLevel.Warning )
        .Enrich.FromLogContext()
        .WriteTo.File( file, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8 )
        .CreateLogger();
    }
}
