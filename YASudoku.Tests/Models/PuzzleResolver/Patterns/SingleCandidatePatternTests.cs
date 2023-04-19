using YASudoku.Models;
using YASudoku.Models.PuzzleResolvers.Patterns;

namespace YASudoku.Tests.Models.PuzzleResolver.Patterns;

public class SingleCandidatePatternTests
{
    public static IEnumerable<object[]> SolvablePuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithMissingSingularCells( out int missingCellsCount );
        yield return new object[] { gameData, missingCellsCount };

        gameData = TestsCommon.CreateValidContainerWithoutMissingCells();
        yield return new object[] { gameData, 0 };
    }

    public static IEnumerable<object[]> UnsolvablePuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithEmptyCells();
        yield return new object[] { gameData, 0 };

        gameData = TestsCommon.CreateValidContainerWithMissingMultipleCells();
        yield return new object[] { gameData, 0 };

        gameData = TestsCommon.CreateValidContainerWithMissingSingularAndMultipleCells( out int solvableCellsCount );
        yield return new object[] { gameData, solvableCellsCount };

        gameData = TestsCommon.CreateValidContainerWithHiddenSingle( out _ );
        yield return new object[] { gameData, 0 };
    }

    [Theory]
    [MemberData( nameof( SolvablePuzzles ) )]
    public void TryResolve_ReturnsTrue_AND_AssignsCorrectFilledCells_WhenGivenResolvablePuzzle( GameDataContainer gameData, int expectedResolvedCells )
    {
        // Arrange
        SingleCandidatePattern pattern = new();
        // Act
        bool result = pattern.TryResolve( gameData, out int actualResolvedCells, new CancellationToken() );
        // Assert
        Assert.True( result );
        Assert.Equal( expectedResolvedCells, actualResolvedCells );
    }

    [Theory]
    [MemberData( nameof( UnsolvablePuzzles ) )]
    public void TryResolve_ReturnsFalse_AND_AssignsCorrectFilledCells_WhenGivenUnsolvablePuzzle( GameDataContainer gameData, int expectedResolvedCells )
    {
        // Arrange
        SingleCandidatePattern pattern = new();
        // Act
        bool result = pattern.TryResolve( gameData, out int actualResolvedCells, new CancellationToken() );
        // Assert
        Assert.False( result );
        Assert.Equal( expectedResolvedCells, actualResolvedCells );
    }
}
