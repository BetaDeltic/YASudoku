namespace YASudoku.Models.PuzzleValidator.Patterns;

public interface IValidatorPattern
{
    bool IsValid( GameDataContainer cells );
}
