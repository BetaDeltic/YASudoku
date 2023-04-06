namespace YASudoku.Models.PuzzleResolver.Patterns;

public class ExposedNTuplePattern : IResolverPattern
{
    /// <summary>
    /// Attempts to help resolve the puzzle by removing candidates for numbers that form N-tuple.
    /// Exposed N-tuple can be a pair, a triple, quad, etc. of same numbers appearing in same amount of cells.
    /// E.g: There are two cells on a row that have a pair of identical candidates
    /// - no other cell in that row can contain those two numbers and can have them removed.
    /// </summary>
    /// <returns>true in case there are no uninitiated cells to be resolved.
    /// false otherwise</returns>
    public bool TryResolve( GameDataContainer gameData, out int affectedCells, CancellationToken cancellationToken )
        => ResolverHelper.RunResolverRepeatedlyOnAllCollections( gameData, TryResolveCollection, out affectedCells, cancellationToken );

    private static bool TryResolveCollection( GameGridCollection collection, out int affectedCells )
    {
        affectedCells = 0;
        bool nothingToSolve =
            ResolverHelper.CheckCellsWithNoValueForEmptyCollection( collection, out var _, out int _ );
        if ( nothingToSolve ) {
            return true;
        }

        IEnumerable<IEnumerable<GameGridCell>> allNTuples = collection
            .GetCellsWithNoValueGroupedByNonSingleCandidates()
            .Where( group => group.Count() == group.First().CandidatesCount ); // Exposed N-tuples have the same amount of candidates as the group size itself

        affectedCells = ( allNTuples
            .Select( nTuple => new {
                candidatesToBeRemoved = nTuple.First().Candidates.ToList(),
                cellsToChange = collection.GetCellsWithNoValue().Where( cell => !nTuple.Contains( cell ) )
            } )
            .SelectMany(
                collectionSelector: t => t.cellsToChange,
                resultSelector: ( t, cell ) => new { cell, t.candidatesToBeRemoved } )
            .Select( t => t.cell.RemoveFromCandidates( t.candidatesToBeRemoved ) ) ).Count( removedCount => removedCount > 0 );

        return false;
    }
}
