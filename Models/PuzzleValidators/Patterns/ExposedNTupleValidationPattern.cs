namespace YASudoku.Models.PuzzleValidators.Patterns;

public class ExposedNTupleValidationPattern : IValidatorPattern
{
    /// <summary>
    /// Validates that the amount of cells that share the same candidates don't exceed the amount of candidates
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns>true if the amount of cells with the same candidates is greater or equal to amount of candidates.
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsValid( GameDataContainer gameData )
        => ValidationHelper.RunValidatorOnAllCollections( gameData, IsValidCollection );

    private bool IsValidCollection( GameGridCollection collection )
    {
        ValidationHelper.ThrowOnEmptyCollection( collection );

        IEnumerable<IEnumerable<GameGridCell>> cellsGroupedByCandidates = collection
            .GetCellsWithNoValueGroupedByNonSingleCandidates();

        foreach( IEnumerable<GameGridCell> group in cellsGroupedByCandidates ) {
            if ( group.First().CandidatesCount < group.Count() ) {
                return false;
            }
        }

        return true;
    }
}
