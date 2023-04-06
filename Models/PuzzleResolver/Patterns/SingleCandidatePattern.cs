using YASudoku.Common;

namespace YASudoku.Models.PuzzleResolver.Patterns;

public class SingleCandidatePattern : IResolverPattern
{
    /// <summary>
    /// Attempts to resolve the puzzle by initializing the missing values for any exposed/open singles.
    /// Exposed/open single happens whenever there is a cell with only one possible candidate.
    /// </summary>
    /// <inheritdoc />
    public bool TryResolve( GameDataContainer gameData, out int resolvedCells, CancellationToken cancellationToken )
        => ResolverHelper.RunResolverRepeatedlyOnAllCollections( gameData, TryResolveCollection, out resolvedCells, cancellationToken );

    private static bool TryResolveCollection( GameGridCollection collection, out int resolvedCells )
    {
        resolvedCells = 0;

        bool nothingToSolve = ResolverHelper.CheckCellsWithNoValueForEmptyCollection( collection,
            out IEnumerable<GameGridCell> uninitiatedCells, out int cellsWithNoValue );
        if ( nothingToSolve ) {
            return true;
        }

        IEnumerable<GameGridCell> solvableCells = uninitiatedCells.Where( cell => cell.CandidatesCount == 1 );

        int solvableCellsCount = solvableCells.Count();

        if ( solvableCellsCount == 0 ) {
            return false;
        }

        solvableCells.ForEach( cell => {
            cell.InitializeWithLastRemainingCandidate();
        } );
        resolvedCells = solvableCellsCount;

        return solvableCellsCount == cellsWithNoValue;
    }
}
