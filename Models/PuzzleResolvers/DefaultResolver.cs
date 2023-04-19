using YASudoku.Models.PuzzleResolvers.Patterns;

namespace YASudoku.Models.PuzzleResolvers;

public class DefaultResolver
{
    private readonly List<IResolverPattern> ResolverPatterns = new();

    private CancellationToken cancellationToken;

    public DefaultResolver( CancellationToken cancellationToken )
    {
        ResolverPatterns.Add( new SingleCandidatePattern() );
        ResolverPatterns.Add( new HiddenSinglePattern() );
        ResolverPatterns.Add( new ExposedNTuplePattern() );
        this.cancellationToken = cancellationToken;
    }

    public void ResetCancellationToken( CancellationToken newCancellationToken )
        => cancellationToken = newCancellationToken;

    public bool TryResolve( GameDataContainer gameData )
    {
        int resolvedInThisCycle;
        do {
            resolvedInThisCycle = 0;
            foreach ( var pattern in ResolverPatterns ) {
                if ( cancellationToken.IsCancellationRequested ) {
                    return false;
                }
                bool success = pattern.TryResolve( gameData, out int resolvedWithThisPattern, cancellationToken );
                resolvedInThisCycle += resolvedWithThisPattern;
                if ( success && !gameData.AllCells.GetCellsWithNoValue().Any() )
                {
                    return true;
                }
            }
        } while ( resolvedInThisCycle > 0 );

        return !gameData.AllCells.GetCellsWithNoValue().Any();
    }
}
