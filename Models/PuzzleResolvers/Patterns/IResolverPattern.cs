namespace YASudoku.Models.PuzzleResolvers.Patterns;

public interface IResolverPattern
{
    /// <param name="gameData"></param>
    /// <param name="resolvedCells"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if the board is fully solved before or after this operation.
    /// false otherwise.</returns>
    bool TryResolve( GameDataContainer gameData, out int resolvedCells, CancellationToken? cancellationToken );
}
