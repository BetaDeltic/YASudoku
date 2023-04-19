namespace YASudoku.Models.PuzzleValidators.Patterns;

public class ExposedSingleCandidateValidationPattern : IValidatorPattern
{
    /// <summary>
    /// Validates that there is at most 1 cell in every row/column/block that has just one specific number,
    /// no other cells are allowed to have the same single candidate or value within the collection.
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns>true if there is at most 1 cell in every row/column/block that has just one specific candidate.
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsValid( GameDataContainer gameData )
        => ValidationHelper.RunValidatorOnAllCollections( gameData, IsValidCollection );

    private bool IsValidCollection( GameGridCollection collection )
    {
        ValidationHelper.ThrowOnEmptyCollection( collection );
        IEnumerable<IEnumerable<int>> allCandidateEnumerables = collection.GetEnumerableOfAllCandidates();
        if ( !allCandidateEnumerables.Any() ) {
            return true;
        }

        IEnumerable<IEnumerable<int>> allCandidateEnumerablesWithSingleCandidate = allCandidateEnumerables
            .Where( candidates => candidates.Count() == 1 );
        if ( !allCandidateEnumerablesWithSingleCandidate.Any() ) {
            return true;
        }

        IEnumerable<int> allSingleCandidates = allCandidateEnumerablesWithSingleCandidate
            .Select( candidates => candidates.First() );

        IEnumerable<int> uniqueCandidates = allSingleCandidates.Distinct();

        if ( allSingleCandidates.Count() != uniqueCandidates.Count() ) {
            return false;
        }

        IEnumerable<GameGridCell> allInitiatedCells = collection.GetInitiatedCells();

        foreach ( GameGridCell cell in allInitiatedCells ) {
            if ( allSingleCandidates.Contains( cell.UserFacingValue ) ) {
                return false;
            }
        }

        return true;
    }
}
