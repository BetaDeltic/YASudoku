using YASudoku.Models;
using YASudoku.ViewModels.GameViewModel.VisualStates;

namespace YASudoku.Services.JournalingServices;

public class PlayerJournalingService : IPlayerJournalingService
{
    private record Transaction( VisualStatesHandler VisualState, PlayerTransactionTypes TransactionType, GameGridCellVisualData AffectedCell, int AffectedNumber )
    {
        public void Rollback()
        {
            if ( TransactionType == PlayerTransactionTypes.RelatedCandidateRemoved ) {
                AffectedCell.ChangeCandidateVisibility( AffectedNumber, setAsVisible: true );
                return;
            }

            VisualState.GameGridVS.UnhighlightCellsWithSameNumber();
            switch ( TransactionType ) {
                case PlayerTransactionTypes.CellValueAdded:
                    VisualState.GameGridVS.SelectNewCell( AffectedCell );
                    VisualState.GameGridVS.RemoveValueFromSelectedCell();
                    break;
                case PlayerTransactionTypes.CellValueChanged:
                    RollbackChangedValue();
                    break;
                case PlayerTransactionTypes.CellValueErased:
                    RollbackChangedValue();
                    break;
                case PlayerTransactionTypes.CandidateAdded:
                    AffectedCell.ChangeCandidateVisibility( AffectedNumber, setAsVisible: false );
                    break;
                case PlayerTransactionTypes.CandidateRemoved:
                    AffectedCell.ChangeCandidateVisibility( AffectedNumber, setAsVisible: true );
                    break;
            }
            PostRollbackHighlighting();
        }

        private void RollbackChangedValue()
        {
            VisualState.GameGridVS.SelectNewCell( AffectedCell );
            VisualState.GameGridVS.ChangeSelectedCellValueAndNotify( AffectedNumber, out _ );
        }

        private void PostRollbackHighlighting()
        {
            VisualState.GameGridVS.HighlightSelectedCell();
            if ( VisualState.SelectedNumber != null ) {
                VisualState.GameGridVS.DeselectCell();
                VisualState.GameGridVS.HighlightCellsWithSameNumber( VisualState.NumPadVS.SelectedButtonNumber );
            }
        }
    }

    private readonly Stack<Transaction> TransactionJournal = new();

    private VisualStatesHandler? VisualState;

    public void SetVisualState( VisualStatesHandler visualStates ) => VisualState = visualStates;

    public void AddTransaction( PlayerTransactionTypes transactionType, GameGridCellVisualData affectedCell, int affectedNumber )
    {
        if ( VisualState == null ) return;

        Transaction transaction = new( VisualState, transactionType, affectedCell, affectedNumber );
        TransactionJournal.Push( transaction );
    }

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
