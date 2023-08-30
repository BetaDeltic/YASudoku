using YASudoku.Models.PuzzleValidators.Patterns;

namespace YASudoku.Models.PuzzleValidators;

public class DefaultValidator : IPuzzleValidator
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
    {
        bool result = true;

        Parallel.ForEach( ValidatorPatterns, ( validatorPattern, loopState ) =>
        {
            if ( !validatorPattern.IsValid( gameData ) ) {
                result = false;
                loopState.Stop();
            }
        } );

        return result;
    }
}
