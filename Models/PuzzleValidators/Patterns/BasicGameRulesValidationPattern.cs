namespace YASudoku.Models.PuzzleValidators.Patterns;

public class BasicGameRulesValidationPattern : IValidatorPattern
{
    /// <summary>
    /// Validates that all the rows, columns and blocks contain every number only once or contains unresolved cells.
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns>true if every collection contains every number only once, or has unresolved cells.
    /// false if any collection contains any number more than once.</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool IsValid( GameDataContainer gameData )
        => ValidationHelper.RunValidatorOnAllCollections( gameData, IsValidCollection );

    private static bool IsValidCollection( GameGridCollection collection )
    {
        ValidationHelper.ThrowOnEmptyCollection( collection );
        return collection.HasDistinctValues();
    }
}
