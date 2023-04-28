using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class TimerVisualState : ObservableObject, IDisposable
{
    private Timer? timer;

    private CancellationTokenSource? gracePeriodCts;

    [ObservableProperty]
    private string _timerText = "00:00";

    private int totalElapsedTime;

    public void Dispose()
    {
        timer?.Dispose();
        GC.SuppressFinalize( this );
    }

    public void StartTimer()
    {
        StopTimerAndSetTextToZero();

        timer = new( TimeSpan.FromSeconds( 1 ) ) { AutoReset = true };

        // Give player a chance to look around a bit
        TimeSpan gracePeriod = TimeSpan.FromSeconds( 1 );
        gracePeriodCts = new CancellationTokenSource();
        StartTimerAfterGracePeriodOrCancelOnPause( gracePeriod, gracePeriodCts.Token );

        // Needs a null check because the timer could have been destroyed before the delay finished
        if ( timer == null ) return;
        timer.Elapsed += Timer_Elapsed;
    }

    private async void StartTimerAfterGracePeriodOrCancelOnPause( TimeSpan gracePeriod, CancellationToken cancellationToken )
    {
        if ( timer == null ) return;
        
        try {
            await Task.Delay( gracePeriod, cancellationToken );
            timer.Start();
        } catch ( TaskCanceledException ) {
            Debug.WriteLine( "Grace period canceled, timer not started" );
        }
    }

    private void Timer_Elapsed( object? sender, System.Timers.ElapsedEventArgs e )
    {
        totalElapsedTime++;
        SetTimerToElapsed();
    }

    private void SetTimerToElapsed()
    {
        TimeSpan elapsedTimeSpan = TimeSpan.FromSeconds( totalElapsedTime );
        TimerText = elapsedTimeSpan.ToString( @"mm\:ss" );
    }

    public void PauseTimer()
    {
        timer?.Stop();
        gracePeriodCts?.Cancel();
    }

    public void StopTimerAndSetTextToZero()
    {
        SetTimerToZero();

        StopAndDisposeTimer();
    }

    private void SetTimerToZero()
    {
        totalElapsedTime = 0;
        SetTimerToElapsed();
    }

    public void StopAndDisposeTimer()
    {
        if ( timer == null ) return;

        timer.Stop();
        timer.Elapsed -= Timer_Elapsed;
        timer.Dispose();
        timer = null;
    }

    public void UnpauseTimer() => timer?.Start();
}
