using YASudoku.Models;
using YASudoku.Models.PuzzleValidator.Patterns;

namespace YASudoku.Tests.Models.PuzzleValidator.Patterns;

public class ExposedNTupleValidationPatternTests
{
    public static IEnumerable<object[]> ValidPuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithEmptyCells();
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
        GameDataContainer gameData = TestsCommon.CreateInvalidContainerWithWrongAmountOfTuples();
        yield return new object[] { gameData };
    }

    [Theory]
    [MemberData( nameof( ValidPuzzles ) )]
    public void IsValid_ReturnsTrue_WhenGivenValidPuzzle( GameDataContainer gameData )
    {
        // Arrange
        ExposedNTupleValidationPattern pattern = new();
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
        ExposedNTupleValidationPattern pattern = new();
        // Act
        bool result = pattern.IsValid( gameData );
        // Assert
        Assert.False( result );
    }
}
