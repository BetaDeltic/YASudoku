using YASudoku.Models;
using YASudoku.Models.PuzzleValidator.Patterns;

namespace YASudoku.Tests.Models.PuzzleValidator.Patterns;

public class BasicGameRulesValidationPatternTests
{
    public const int gridSize = 9;
    public const int blockSize = 3;

    public static IEnumerable<object[]> ValidPuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithEmptyCells();
        yield return new object[] { gameData };

        gameData = TestsCommon.CreateValidContainerWithMissingSingularCells( out int _ );
        yield return new object[] { gameData };

        gameData = TestsCommon.CreateValidContainerWithoutMissingCells();
        yield return new object[] { gameData };

        gameData = TestsCommon.CreateValidContainerWithMissingMultipleCells();
        yield return new object[] { gameData };
    }

    public static IEnumerable<object[]> InvalidPuzzles()
    {
        GameDataContainer gameData = TestsCommon.InitInvalidContainerWithValueAppearingMoreThanOnceSingularCells();
        yield return new object[] { gameData };
    }

    [Theory]
    [MemberData( nameof( ValidPuzzles ) )]
    public void IsValid_ReturnsTrue_WhenGivenValidPuzzle( GameDataContainer gameData )
    {
        // Arrange
        BasicGameRulesValidationPattern pattern = new();
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
        BasicGameRulesValidationPattern pattern = new();
        // Act
        bool result = pattern.IsValid( gameData );
        // Assert
        Assert.False( result );
    }
}
