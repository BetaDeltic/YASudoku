namespace YASudoku.Models.PuzzleValidator.Patterns;

public class HiddenSingleValidationPattern : IValidatorPattern
{
    /// <summary>
    /// Validates that there are no single cells with multiple hidden singles.
    /// E.g. the only cell on a row that can have 1 and also the only one that can have 2, if no other cell has these candidates - the puzzle is invalid
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns>true if there there are no single cells with multiple hidden singles.
    /// false otherwise.</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsValid( GameDataContainer gameData )
        => ValidationHelper.RunValidatorOnAllCollections( gameData, IsValidCollection );

    private static bool IsValidCollection( GameGridCollection collection )
    {
        ValidationHelper.ThrowOnEmptyCollection( collection );

        Dictionary<int, int> candidateCounts = collection.GetCandidateCounts();

        IEnumerable<int> singleCandidates = candidateCounts
            .Where( keyValuePair => keyValuePair.Value == 1 )
            .Select( keyValuePair => keyValuePair.Key );

        return singleCandidates
            .Select( collection.GetCellsWithSpecificCandidate )
            .All( cellsWithCandidate => cellsWithCandidate.Count <= 1 );
    }
}
