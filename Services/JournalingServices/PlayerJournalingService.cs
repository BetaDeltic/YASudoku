using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Services.JournalingServices;

public class PlayerJournalingService : IPlayerJournalingService
{
    internal record Transaction( VisualStatesHandler VisualState, PlayerTransactionTypes TransactionType,
        GameGridCellVisualData AffectedCell, int AffectedNumber )
    {
        public void Rollback()
        {
            if ( TransactionType == PlayerTransactionTypes.RelatedCandidateRemoved ) {
                AffectedCell.AddCandidate( AffectedNumber, addToJournal: false );
                return;
            }

            VisualState.GameGridVS.UnhighlightCellsWithSameNumber();
            VisualState.GameGridVS.SelectNewCell( AffectedCell );

            switch ( TransactionType ) {
                case PlayerTransactionTypes.CellValueAdded:
                    VisualState.GameGridVS.RemoveValueFromSelectedCell( addToJournal: false );
                    break;
                case PlayerTransactionTypes.CellValueChanged:
                    VisualState.GameGridVS.ChangeSelectedCellValueAndNotify( AffectedNumber, addToJournal: false );
                    break;
                case PlayerTransactionTypes.CellValueErased:
                    VisualState.GameGridVS.ChangeSelectedCellValueAndNotify( AffectedNumber, addToJournal: false );
                    break;
                case PlayerTransactionTypes.CandidateAdded:
                    AffectedCell.RemoveCandidate( AffectedNumber, addToJournal: false );
                    break;
                case PlayerTransactionTypes.CandidateRemoved:
                    AffectedCell.AddCandidate( AffectedNumber, addToJournal: false );
                    break;
                case PlayerTransactionTypes.RelatedCandidateRemoved:
                    break;
            }
            PostRollbackHighlighting();
        }

        private void PostRollbackHighlighting()
        {
            VisualState.GameGridVS.HighlightSelectedCell();

            if ( VisualState.SelectedNumber == null ) return;

            VisualState.GameGridVS.DeselectCell();
            VisualState.GameGridVS.HighlightCellsWithSameNumber( VisualState.NumPadVS.SelectedButtonNumber );
        }
    }

    internal readonly Stack<Transaction> TransactionJournal = new();

    private VisualStatesHandler? VisualState;

    public void SetVisualState( VisualStatesHandler visualStates ) => VisualState = visualStates;

    public void AddTransaction( PlayerTransactionTypes transactionType, GameGridCellVisualData affectedCell, int affectedNumber )
    {
        if ( VisualState == null ) return;

        Transaction transaction = new( VisualState, transactionType, affectedCell, affectedNumber );
        TransactionJournal.Push( transaction );
    }

    /// <summary>
    /// Rolls back one transaction, in case the transaction is a related candidate removal, it will roll back all related candidate removals.
    /// </summary>
    public void ReverseTransaction()
    {
        while ( TransactionJournal.TryPeek( out Transaction? transaction ) ) {
            TransactionJournal.Pop().Rollback();
            if ( transaction.TransactionType != PlayerTransactionTypes.RelatedCandidateRemoved ) {
                break;
            }
        }
    }

    public void ClearJournal() => TransactionJournal.Clear();
}
