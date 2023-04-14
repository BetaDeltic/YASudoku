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

    public void StartTimer()
    {
        totalElapsedTime = 0;
        if ( timer != null ) {
            timer.Stop();
            timer.Elapsed -= Timer_Elapsed;
            timer.Dispose();
        }

        timer = new( TimeSpan.FromSeconds( 1 ) ) {
            AutoReset = true,
            Enabled = true,
        };
        timer.Elapsed += Timer_Elapsed;
    }

    private void Timer_Elapsed( object? sender, System.Timers.ElapsedEventArgs e )
    {
        totalElapsedTime++;
        TimeSpan elapsedTimeSpan = TimeSpan.FromSeconds( totalElapsedTime );
        TimerText = elapsedTimeSpan.ToString( @"mm\:ss" );
    }

    public void UnpauseTimer() => timer?.Start();

    public void StopTimer() => timer?.Stop();
}
