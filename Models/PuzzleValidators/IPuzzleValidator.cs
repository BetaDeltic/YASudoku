namespace YASudoku.Models.PuzzleValidators;

public interface IPuzzleValidator
{
    public bool IsValid( GameDataContainer gameData );
}
