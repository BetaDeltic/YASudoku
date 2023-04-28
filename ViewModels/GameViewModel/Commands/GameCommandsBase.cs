using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class GameCommandsBase
{
    private readonly IPlayerJournalingService journal;
    protected readonly VisualStatesHandler visualState;

    public GameCommandsBase( VisualStatesHandler visualState, IPlayerJournalingService journalingService )
    {
        this.visualState = visualState;
        journal = journalingService;
    }

    public async Task EndRunningGame()
    {
        visualState.TimerVS.StopTimerAndSetTextToZero();
        visualState.SignalWhenWipingGameBoard.OnNext( Unit.Default );
        await AwaitBehaviorSubjectAndResetIt( visualState.WipingGameBoardCompleted );
    }

    public async Task StartNewGame()
    {
        journal.ClearJournal();
        visualState.ResetVisualStatesToDefault();
        visualState.SignalWhenStartingNewGame.OnNext( Unit.Default );
        await AwaitBehaviorSubjectAndResetIt( visualState.StartingNewGameCompleted );
        visualState.StartGame();
    }

    private static async Task AwaitBehaviorSubjectAndResetIt( BehaviorSubject<bool> behaviorSubject )
    {
        await behaviorSubject.FirstAsync( value => value == true );
        behaviorSubject.OnNext( false );
    }
}
