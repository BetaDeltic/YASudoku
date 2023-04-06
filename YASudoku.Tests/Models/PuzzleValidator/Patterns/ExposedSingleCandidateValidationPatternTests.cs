using YASudoku.Models;
using YASudoku.Models.PuzzleValidator.Patterns;

namespace YASudoku.Tests.Models.PuzzleValidator.Patterns;

public class ExposedSingleCandidateValidationPatternTests
{
    public static IEnumerable<object[]> ValidPuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithEmptyCells();
        yield return new object[] { gameData };

        TestsCommon.InitializeCollectionWithSpecificSequence( gameData.ByRows[ 0 ], Enumerable.Range( 1, 8 ) );
        yield return new object[] { gameData };

        gameData = TestsCommon.CreateValidContainerWithoutMissingCells();
        yield return new object[] { gameData };

        gameData = TestsCommon.CreateValidContainerWithMissingSingularCells( out int _ );
        yield return new object[] { gameData };

        gameData = TestsCommon.CreateValidContainerWithMissingMultipleCells();
        yield return new object[] { gameData };

        gameData = TestsCommon.CreateValidContainerWithMissingSingularAndMultipleCells( out int _ );
        yield return new object[] { gameData };
    }

    public static IEnumerable<object[]> InvalidPuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithEmptyCells();
        TestsCommon.InitializeCollectionWithSpecificSequence( gameData.ByRows[ 0 ], Enumerable.Range( 1, 7 ) );
        gameData.ByRows[ 0 ][ ^1 ].RemoveFromCandidates( 8 );

        gameData.ByRows[ 0 ][ ^2 ].ResetCell();
        for ( int i = 1; i < 9; i++ ) {
            gameData.ByRows[ 0 ][ ^2 ].RemoveFromCandidates( i );
        }

        yield return new object[] { gameData };
    }

    [Theory]
    [MemberData( nameof( ValidPuzzles ) )]
    public void IsValid_ReturnsTrue_WhenGivenValidPuzzle( GameDataContainer gameData )
    {
        // Arrange
        ExposedSingleCandidateValidationPattern pattern = new();
        // Act
        bool result = pattern.IsValid( gameData );
        // Assert
        Assert.True( result );
    }

    [Theory]
    [MemberData( nameof( InvalidPuzzles ) )]
    public void IsValid_ReturnFalse_WhenGivenInvalidPuzzle( GameDataContainer gameData )
    {
        // Arrange
        ExposedSingleCandidateValidationPattern pattern = new();
        // Act
        bool result = pattern.IsValid( gameData );
        // Assert
        Assert.False( result );
    }
}
