using YASudoku.Models.PuzzleValidators.Patterns;

namespace YASudoku.Models.PuzzleValidators;

public class DefaultValidator
{
    private readonly List<IValidatorPattern> ValidatorPatterns = new();

    public DefaultValidator()
    {
        ValidatorPatterns.Add( new BasicGameRulesValidationPattern() );
        ValidatorPatterns.Add( new ExposedSingleCandidateValidationPattern() );
        ValidatorPatterns.Add( new ExposedNTupleValidationPattern() );
        ValidatorPatterns.Add( new HiddenSingleValidationPattern() );
    }

    public bool IsValid( GameDataContainer gameData )
        => ValidatorPatterns.All( validatorPattern => validatorPattern.IsValid( gameData ) );
}
