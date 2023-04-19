namespace YASudoku.Models.PuzzleResolvers.Patterns;

public static class ResolverHelper
{
    public delegate bool ResolverFunc( GameGridCollection collection, out int resolved );

    public static bool RunResolverRepeatedlyOnAllCollections(
        GameDataContainer gameData, ResolverFunc func, out int totalResolved, CancellationToken cancellationToken )
    {
        totalResolved = 0;
        bool success;
        int resolvedInThisCycle;
        do {
            success = true;
            resolvedInThisCycle = 0;
            for ( int i = 0; i < gameData.ByRows.Count; i++ ) {
                RunOverCollection( func, gameData.ByRows[ i ], ref totalResolved, ref success, ref resolvedInThisCycle,
                    cancellationToken );
                RunOverCollection( func, gameData.ByColumns[ i ], ref totalResolved, ref success, ref resolvedInThisCycle,
                    cancellationToken );
                RunOverCollection( func, gameData.ByBlocks[ i ], ref totalResolved, ref success, ref resolvedInThisCycle,
                    cancellationToken );
            }
        } while ( resolvedInThisCycle > 0 );

        return success;
    }

    private static void RunOverCollection( ResolverFunc func, GameGridCollection collection, ref int totalResolved,
        ref bool success, ref int resolvedInThisCycle, CancellationToken cancellationToken )
    {
        if ( cancellationToken.IsCancellationRequested )
            return;

        if ( !func( collection, out int resolved ) ) {
            success = false;
        }
        totalResolved += resolved;
        resolvedInThisCycle += resolved;
    }

    public static bool CheckCellsWithNoValueForEmptyCollection( GameGridCollection collection,
        out IEnumerable<GameGridCell> uninitiatedCells, out int uninitiatedCount )
    {
        uninitiatedCells = collection.GetCellsWithNoValue();
        uninitiatedCount = uninitiatedCells.Count();

        return uninitiatedCount == 0;
    }
}
