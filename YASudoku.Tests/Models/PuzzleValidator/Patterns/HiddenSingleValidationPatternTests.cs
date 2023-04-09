using YASudoku.Models;
using YASudoku.Models.PuzzleValidator.Patterns;

namespace YASudoku.Tests.Models.PuzzleValidator.Patterns;

public class HiddenSingleValidationPatternTests
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

        // specific case found by bug
        gameData = TestsCommon.CreateValidContainerWithEmptyCells();
        TestsCommon.InitializeCollectionWithSpecificSequence( gameData.ByRows[ 0 ], new List<int> { 2, 3, 7, 5 } );
        gameData.ByRows[ 0 ][ 4 ].RemoveFromCandidates( new List<int> { 1, 6, 8 } ); // Leaves only 9 and 4
        gameData.ByRows[ 0 ][ 5 ].RemoveFromCandidates( new List<int> { 4, 6, 8 } ); // Leaves only 1 and 9
        gameData.ByRows[ 0 ][ 6 ].RemoveFromCandidates( new List<int> { 4, 6, 9 } ); // Leaves only 8 and 1
        gameData.ByRows[ 0 ][ 7 ].RemoveFromCandidates( new List<int> { 1, 4 } ); // Leaves only 6,8,9
        gameData.ByRows[ 0 ][ 8 ].RemoveFromCandidates( new List<int> { 4, 8 } ); // Leaves only 6,1,9
        
        yield return new object[] { gameData };
    }

    public static IEnumerable<object[]> InvalidPuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithEmptyCells();
        TestsCommon.InitializeCollectionWithSpecificSequence( gameData.ByRows[ 0 ], Enumerable.Range( 1, 7 ) );
        gameData.ByRows[ 0 ][ ^2 ].Initialize( 7 );

        yield return new object[] { gameData };
    }

    [Theory]
    [MemberData( nameof( ValidPuzzles ) )]
    public void IsValid_ReturnsTrue_WhenGivenValidPuzzle( GameDataContainer gameData )
    {
        // Arrange
        HiddenSingleValidationPattern pattern = new();
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
        HiddenSingleValidationPattern pattern = new();
        // Act
        bool result = pattern.IsValid( gameData );
        // Assert
        Assert.False( result );
    }
}
