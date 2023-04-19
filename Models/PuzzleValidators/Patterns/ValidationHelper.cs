namespace YASudoku.Models.PuzzleValidators.Patterns;

public class ValidationHelper
{
    public static bool RunValidatorOnAllCollections( GameDataContainer gameData, Func<GameGridCollection, bool> func )
    {
        foreach ( GameGridCollection collection in gameData.ByRows ) {
            if ( !func( collection ) ) {
                return false;
            }
        };

        foreach ( GameGridCollection collection in gameData.ByColumns ) {
            if ( !func( collection ) ) {
                return false;
            }
        };

        foreach ( GameGridCollection collection in gameData.ByBlocks ) {
            if ( !func( collection ) ) {
                return false;
            }
        };

        return true;
    }

    public static void ThrowOnEmptyCollection( GameGridCollection collection )
    {
        if ( collection.Count == 0 ) {
            throw new ArgumentException( $"Supplied {nameof( collection )} can NOT be empty.", nameof( collection ) );
        }
    }
}
