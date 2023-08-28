using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Services.JournalingServices;

public enum PlayerTransactionTypes { CellValueAdded, CellValueChanged, CellValueErased, CandidateAdded, CandidateRemoved, RelatedCandidateRemoved }

public interface IPlayerJournalingService
{
    void SetVisualState( VisualStatesHandler visualStates );
    void AddTransaction( PlayerTransactionTypes transactionType, GameGridCellVisualData affectedCell, int affectedNumber );
    void ReverseTransaction();
    void ClearJournal();
}
