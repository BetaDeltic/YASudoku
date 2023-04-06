using YASudoku.Models;
using YASudoku.Models.PuzzleResolver.Patterns;

namespace YASudoku.Tests.Models.PuzzleResolver.Patterns;

public class ExposedNTuplePatternTests
{
    public static IEnumerable<object[]> SolvablePuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithoutMissingCells();
        yield return new object[] { gameData, 0 };
    }

    public static IEnumerable<object[]> UnsolvablePuzzles()
    {
        GameDataContainer gameData = TestsCommon.CreateValidContainerWithEmptyCells();
        yield return new object[] { gameData, 0 };

        gameData = TestsCommon.CreateValidContainerWithMissingMultipleCells();
        yield return new object[] { gameData, 0 };

        gameData = TestsCommon.CreateValidContainerWithMissingSingularAndMultipleCells( out var _ );
        yield return new object[] { gameData, 0 };

        gameData = TestsCommon.CreateValidContainerWithExposedNTuples( out int solvableCellsCount );
        yield return new object[] { gameData, solvableCellsCount };
    }

    [Theory]
    [MemberData( nameof( SolvablePuzzles ) )]
    public void TryResolve_ReturnsTrue_AND_AssignsCorrectAffectedCells_WhenGivenResolvablePuzzle( GameDataContainer gameData, int expectedAffectedCells )
    {
        // Arrange
        ExposedNTuplePattern pattern = new();
        // Act
        bool result = pattern.TryResolve( gameData, out int actualAffectedCells, new CancellationToken() );
        // Assert
        Assert.True( result );
        Assert.Equal( expectedAffectedCells, actualAffectedCells );
    }

    [Theory]
    [MemberData( nameof( UnsolvablePuzzles ) )]
    public void TryResolve_ReturnsFalse_AND_AssignsCorrectAffectedCells_WhenGivenUnsolvablePuzzle( GameDataContainer gameData, int expectedAffectedCells )
    {
        // Arrange
        ExposedNTuplePattern pattern = new();
        // Act
        gameData.DebugPrintGeneratedPuzzle( "Puzzle Before" );
        bool result = pattern.TryResolve( gameData, out int actualAffectedCells, new CancellationToken() );
        gameData.DebugPrintGeneratedPuzzle( "Puzzle After" );
        // Assert
        Assert.False( result );
        Assert.Equal( expectedAffectedCells, actualAffectedCells );
    }
}
