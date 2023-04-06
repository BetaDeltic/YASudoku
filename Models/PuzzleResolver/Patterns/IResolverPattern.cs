namespace YASudoku.Models.PuzzleResolver.Patterns;

public interface IResolverPattern
{
    /// <param name="gameData"></param>
    /// <param name="resolvedCells"></param>
    /// <returns>true if the board is fully solved before or after this operation.
    /// false otherwise.</returns>
    bool TryResolve( GameDataContainer gameData, out int resolvedCells, CancellationToken cancellationToken );
}
