using YASudoku.Models;

namespace YASudoku.Services.JournalingServices;

public class GeneratorJournalingService : IGeneratorJournalingService
{
    private enum TransactionStates { ReadyForNewTransaction, PendingTransactionDetails, ReadyForCompletion }

    private record Transaction(
        GeneratorTransactionTypes TransactionType,
        GameGridCell AffectedCell,
        int AffectedNumber,
        bool DeleteCandidateOnRollback = false )
    {
        private readonly Stack<Transaction> SubTransactions = new();

        public void AddSubTransaction( Transaction transaction ) => SubTransactions.Push( transaction );

        public void Rollback()
        {
            int SubTransactionsCount = SubTransactions.Count;

            for ( int i = 0; i < SubTransactionsCount; i++ ) {
                Transaction transaction = SubTransactions.Pop();
                transaction.Rollback();
            }

            switch ( TransactionType ) {
                case GeneratorTransactionTypes.CellValueInitialized:
                    AffectedCell.ReverseInitialization();
                    if ( DeleteCandidateOnRollback ) {
                        AffectedCell.RemoveFromCandidates( AffectedNumber );
                    }
                    break;
                case GeneratorTransactionTypes.CandidateRemoved:
                    AffectedCell.AddToCandidates( AffectedNumber );
                    break;
            }
        }
    };

    private bool HasActiveTransaction => TransactionState != TransactionStates.ReadyForNewTransaction;

    private readonly Stack<Transaction> TransactionJournal = new();
    private TransactionStates TransactionState = TransactionStates.ReadyForNewTransaction;
    private Transaction? pendingTransaction = null;

    public void StartTransaction()
    {
        if ( HasActiveTransaction ) {
            throw new InvalidOperationException( "Can't start a new transaction while there is an active transaction pending!" );
        }

        TransactionState = TransactionStates.PendingTransactionDetails;
    }

    public void AddSubTransaction( GeneratorTransactionTypes transactionType, GameGridCell affectedCell, int affectedNumber )
    {
        if ( !HasActiveTransaction ) {
            throw new InvalidOperationException( $"Can't add a subtransaction while there is no active transaction pending!" +
                $"substransaction details: (type:{Enum.GetName( transactionType )}, cellID: {affectedCell.CellID}, number:{affectedNumber})" );
        }

        if ( TransactionState == TransactionStates.PendingTransactionDetails ) {
            pendingTransaction = new Transaction( transactionType, affectedCell, affectedNumber, DeleteCandidateOnRollback: true );
            TransactionState = TransactionStates.ReadyForCompletion;
        } else {
            pendingTransaction!.AddSubTransaction( new Transaction( transactionType, affectedCell, affectedNumber ) );
        }
    }

    public void CommitTransaction()
    {
        if ( TransactionState != TransactionStates.ReadyForCompletion ) {
            throw new InvalidOperationException( $"Can't commit a transaction while there is no active transaction pending!" );
        }

        TransactionJournal.Push( pendingTransaction! );
        pendingTransaction = null;

        TransactionState = TransactionStates.ReadyForNewTransaction;
    }

    public void ReverseTransaction()
    {
        if ( TransactionState == TransactionStates.ReadyForCompletion ) {
            pendingTransaction!.Rollback();
            pendingTransaction = null;
        } else {
            TransactionJournal.Pop().Rollback();
        }

        TransactionState = TransactionStates.ReadyForNewTransaction;
    }

    public void ClearJournal()
    {
        TransactionJournal.Clear();
        TransactionState = TransactionStates.ReadyForNewTransaction;
        pendingTransaction = null;
    }
}
