using YASudoku.Services.JournalingServices;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class UndoCmd : CommandsBase
{
    private readonly IPlayerJournalingService journal;

    public UndoCmd( IPlayerJournalingService journalingService, VisualStatesHandler visualState ) : base( visualState )
    {
        journal = journalingService;
    }

    public void UndoLastActionInJournal()
    {
        if ( IsPaused ) return;

        journal.ReverseTransaction();
    }
}
