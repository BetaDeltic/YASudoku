using YASudoku.Services.JournalingServices;

namespace YASudoku.ViewModels.GameViewModel.Commands;

public class UndoCmd
{
    private readonly IPlayerJournalingService journal;

    public UndoCmd( IPlayerJournalingService journalingService )
    {
        journal = journalingService;
    }

    public void UndoLastActionInJournal() => journal.ReverseTransaction();
}
