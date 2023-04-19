namespace YASudoku.Models.PuzzleResolvers.Patterns;

public class HiddenSinglePattern : IResolverPattern
{
    /// <summary>
    /// Attempts to resolve the puzzle by initializing the missing values for any hidden singles.
    /// Hidden single happens whenever a number can appear only in one possible place in a row/column/block.
    /// </summary>
    /// <inheritdoc />
    public bool TryResolve( GameDataContainer gameData, out int resolvedCells, CancellationToken cancellationToken )
        => ResolverHelper.RunResolverRepeatedlyOnAllCollections( gameData, TryResolveCollection, out resolvedCells, cancellationToken );

    private static bool TryResolveCollection( GameGridCollection collection, out int resolvedCells )
    {
        resolvedCells = 0;

        bool nothingToSolve = ResolverHelper.CheckCellsWithNoValueForEmptyCollection( collection, out var _, out int cellsWithNoValue );
        if ( nothingToSolve ) {
            return true;
        }

        Dictionary<int, int> candidateCounts = collection.GetCandidateCounts();

        IEnumerable<int> singleCandidates = candidateCounts
            .Where( keyValuePair => keyValuePair.Value == 1 )
            .Select( keyValuePair => keyValuePair.Key );

        foreach ( int candidate in singleCandidates ) {
            List<GameGridCell> cellsWithCandidate = collection.GetCellsWithSpecificCandidate( candidate );
            if ( cellsWithCandidate.Count != 1 ) continue;

            GameGridCell impactedCell = cellsWithCandidate.First();
            impactedCell.Initialize( candidate );
            resolvedCells++;
        }

        return cellsWithNoValue == resolvedCells;
    }
}
