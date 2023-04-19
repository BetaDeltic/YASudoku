namespace YASudoku.Models.PuzzleValidators.Patterns;

public interface IValidatorPattern
{
    bool IsValid( GameDataContainer cells );
}
