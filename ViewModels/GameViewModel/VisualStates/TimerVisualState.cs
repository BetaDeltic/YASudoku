using CommunityToolkit.Mvvm.ComponentModel;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public partial class TimerVisualState : ObservableObject, IDisposable
{
    private System.Timers.Timer? timer;

    [ObservableProperty]
    private string _timerText = "00:00";

    private int totalElapsedTime;

    public void Dispose()
    {
        timer?.Dispose();
        GC.SuppressFinalize( this );
    }

    public async void StartTimer()
    {
        StopTimerAndSetTextToZero();

        timer = new( TimeSpan.FromSeconds( 1 ) ) { AutoReset = true };

        TimeSpan gracePeriod = TimeSpan.FromSeconds( 1 ); // Give player a chance to look around a bit
        await Task.Delay( gracePeriod ).ContinueWith( _ => timer?.Start() );

        // Needs a null check because the timer could have been destroyed before the delay finished
        if ( timer == null ) return;
        timer.Elapsed += Timer_Elapsed;
    }

    private void SetTimerToZero()
    {
        totalElapsedTime = 0;
        SetTimerToElapsed();
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

    public void PauseTimer() => timer?.Stop();

    public void StopTimerAndSetTextToZero()
    {
        if ( timer == null ) return;

        SetTimerToZero();

        StopAndDisposeTimer();
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
