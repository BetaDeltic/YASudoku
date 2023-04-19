namespace YASudoku.Models.PuzzleResolvers;

public interface IPuzzleResolver
{
    public void SetCancellationToken( CancellationToken newCancellationToken );
    public bool TryResolve( GameDataContainer gameData );
}
