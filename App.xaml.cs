﻿using Serilog;
using System.Runtime.ExceptionServices;
using YASudoku.Common;

namespace YASudoku;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

        UserAppTheme = AppTheme.Dark;
    }

    private static void CurrentDomain_FirstChanceException( object? sender, FirstChanceExceptionEventArgs e )
        => ExceptionLogging.LogException( e.Exception );

    private static void CurrentDomain_UnhandledException( object sender, UnhandledExceptionEventArgs e )
    {
        ExceptionLogging.LogException( e.ExceptionObject );

        Log.CloseAndFlush();
        Environment.Exit( 0 );
    }

#if WINDOWS
    protected override Window CreateWindow( IActivationState? activationState )
    {
        Window window = new(new AppShell());
        window.Activated += Window_Activated;
        return window;
    }

    private static async void Window_Activated( object? sender, EventArgs e )
    {
        if ( sender is not Window window ) return;

        const int DefaultWidth = 550; // 550; 454
        const int DefaultHeight = 820; // 820; 680

        // change window size.
        window.Width = DefaultWidth;
        window.Height = DefaultHeight;

        // give it some time to complete window resizing task.
        await window.Dispatcher.DispatchAsync( () => { } );

        DisplayInfo display = DeviceDisplay.Current.MainDisplayInfo;

        // move to screen center
        window.X = ( display.Width / display.Density - window.Width ) / 2;
        window.Y = ( display.Height / display.Density - window.Height ) / 2;

        window.MaximumHeight = DefaultHeight;
        window.MinimumHeight = DefaultHeight;
        window.MaximumWidth = DefaultWidth;
        window.MinimumWidth = DefaultWidth;

        window.Activated -= Window_Activated;
    }

#elif ANDROID
    protected override Window CreateWindow( IActivationState? activationState )
    {
        Window window = new( new AppShell() );
        return window;
    }
#endif
}
