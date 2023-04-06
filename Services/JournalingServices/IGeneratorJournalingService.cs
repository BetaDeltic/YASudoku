using YASudoku.Models;

namespace YASudoku.Services.JournalingServices;

public enum GeneratorTransactionTypes { CellValueInitialized, CandidateRemoved }

public interface IGeneratorJournalingService
{
    void StartTransaction();
    void AddSubTransaction( GeneratorTransactionTypes transactionType, GameGridCell affectedCell, int affectedNumber );
    void CommitTransaction();
    void ReverseTransaction();
    void ClearJournal();
}
