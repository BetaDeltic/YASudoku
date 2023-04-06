using YASudoku.Models.PuzzleValidator.Patterns;

namespace YASudoku.Models.PuzzleValidator;

public class PuzzleValidator
{
    private readonly List<IValidatorPattern> ValidatorPatterns = new();

    public PuzzleValidator()
    {
        ValidatorPatterns.Add( new BasicGameRulesValidationPattern() );
        ValidatorPatterns.Add( new ExposedSingleCandidateValidationPattern() );
        ValidatorPatterns.Add( new ExposedNTupleValidationPattern() );
        ValidatorPatterns.Add( new HiddenSingleValidationPattern() );
    }

    public bool IsValid( GameDataContainer gameData )
        => ValidatorPatterns.All( validatorPattern => validatorPattern.IsValid( gameData ) );
}
