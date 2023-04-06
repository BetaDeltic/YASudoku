using YASudoku.Models;
using YASudoku.Services.JournalingServices;
using YASudoku.Models.PuzzleGenerator;
using YASudoku.Services.SettingsService;

namespace YASudoku.Tests.Models.PuzzleGenerator;

public class TraditionGeneratorTests
{
    public const int gridSize = 9;
    public TraditionalGenerator? generator;

    [Fact (Skip = "To be run only manually, takes multiple minutes to finish")]
    public void AllGeneratedPuzzlesAreCorrect()
    {
        for ( int i = 0; i < 10000; i++ ) {
            // Arrange
            generator = new( new GeneratorJournalingService(), new SettingsService() );
            // Act
            GameDataContainer gameData = generator.GenerateNewPuzzle();

            // Assert
            Assert.NotNull( gameData );
            gameData.DebugPrintGeneratedPuzzle( $"Generated puzzle #{i}:" );
            gameData.ByRows.ForEach( row => AssertPuzzleCollectionContainsEveryNumberOnce( row.GetAllCellValues() ) );
            gameData.ByColumns.ForEach( column => AssertPuzzleCollectionContainsEveryNumberOnce( column.GetAllCellValues() ) );
            gameData.ByBlocks.ForEach( block => AssertPuzzleCollectionContainsEveryNumberOnce( block.GetAllCellValues() ) );
        }
    }

    private static void AssertPuzzleCollectionContainsEveryNumberOnce( IEnumerable<int> collection )
    {
        IEnumerable<int> allPossibleNumbers = Enumerable.Range( 1, gridSize );
        IEnumerable<int> sharedNumbers = allPossibleNumbers.Intersect( collection );

        Assert.True( sharedNumbers.Count() == allPossibleNumbers.Count() );
    }
}
