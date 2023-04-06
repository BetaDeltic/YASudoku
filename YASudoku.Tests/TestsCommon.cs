﻿using Microsoft.Maui.Graphics;
using Moq;
using YASudoku.Models;
using YASudoku.Services.JournalingServices;
using YASudoku.Services.SettingsService;

namespace YASudoku.Tests;

public class TestsCommon
{
    public static GameDataContainer CreateGameDataWithSpecificSequence( IEnumerable<int> sequence )
    {
        GameDataContainer gameData = new( 9, GetJournalMock(), GetSettingsMock() );
        gameData.InitializeCellCollections();

        InitializeCollectionWithSpecificSequence( gameData.AllCells, sequence );

        return gameData;
    }

    public static void InitializeCollectionWithSpecificSequence( GameGridCollection collection, IEnumerable<int> sequence )
    {
        if ( sequence.Count() > collection.Count ) {
            throw new ArgumentException( $"Amount of values in {nameof( sequence )} can't be larger than amount of cells in {nameof( collection )} collection." );
        }

        IEnumerable<int> positiveNumbers = sequence.Where( number => number > 0 );
        if ( !positiveNumbers.Any() ) {
            return;
        }

        foreach ( (int number, int index) in sequence.Select( ( number, index ) => (number, index) ) ) {
            if ( number > 0 ) {
                collection[ index ].Initialize( number );
            }
        }
    }

    public static GameDataContainer CreateValidContainerWithEmptyCells()
    {
        GameDataContainer gameData = new( 9, GetJournalMock(), GetSettingsMock() );
        gameData.InitializeCellCollections();
        return gameData;
    }

    public static GameDataContainer CreateValidContainerWithoutMissingCells()
    {
        List<int> sequence = new() {
                1,2,3,4,5,6,7,8,9,
                4,5,6,7,8,9,1,2,3,
                7,8,9,1,2,3,4,5,6,
                2,3,4,5,6,7,8,9,1,
                5,6,7,8,9,1,2,3,4,
                8,9,1,2,3,4,5,6,7,
                3,4,5,6,7,8,9,1,2,
                6,7,8,9,1,2,3,4,5,
                9,1,2,3,4,5,6,7,8
            };

        return CreateGameDataWithSpecificSequence( sequence );
    }

    public static GameDataContainer CreateValidContainerWithMissingSingularCells( out int solvableCellsCount )
    {
        List<int> sequence = new() {
                1,2,0,4,5,6,0,8,9,
                4,5,6,7,8,9,1,2,3,
                7,8,9,1,2,3,4,5,6,
                2,3,4,5,6,7,8,9,1,
                5,6,0,8,0,1,2,3,4,
                8,9,1,2,3,0,5,6,7,
                3,4,5,6,7,8,9,1,2,
                6,0,8,9,1,2,3,4,5,
                9,1,2,3,4,5,6,7,0
            };
        solvableCellsCount = CountZeroesInSequence( sequence );

        return CreateGameDataWithSpecificSequence( sequence );
    }

    public static GameDataContainer CreateValidContainerWithMissingMultipleCells()
    {
        List<int> sequence = new() {
                0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,
                2,3,4,5,6,7,8,9,1,
                5,6,7,8,9,1,2,3,4,
                8,9,1,2,3,4,5,6,7,
                3,4,5,6,7,8,9,1,2,
                6,7,8,9,1,2,3,4,5,
                9,1,2,3,4,5,6,7,8
            };
        return CreateGameDataWithSpecificSequence( sequence );
    }

    public static GameDataContainer CreateValidContainerWithMissingSingularAndMultipleCells( out int solvableCellsCount )
    {
        List<int> sequence = new() {
                0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,0,
                2,3,4,5,6,7,8,9,1,
                5,6,7,8,0,1,2,3,4,
                8,9,1,2,3,0,5,6,7,
                3,4,5,6,7,8,9,1,2,
                6,0,8,9,1,2,3,4,5,
                9,1,2,3,4,5,6,7,0
            };
        GameDataContainer gameData = CreateGameDataWithSpecificSequence( sequence );

        solvableCellsCount = gameData.ByRows.Aggregate( 0, ( total, next ) => {
            total += GetCellsWithOneCandidate( next ).Count();
            return total;
        } );

        return gameData;
    }

    public static GameDataContainer CreateValidContainerWithHiddenSingle( out int solvableCellsCount )
    {
        List<int> sequence = new() {
                1,2,0,0,0,0,0,0,0,
                0,0,0,0,0,3,0,0,0,
                0,0,0,0,0,0,0,0,3,
                0,3,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,3,0,
                0,0,0,0,3,0,0,0,0,
                3,0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,3,0,0,
                0,0,0,3,0,0,0,0,0
            };
        solvableCellsCount = 1;

        return CreateGameDataWithSpecificSequence( sequence );
    }

    public static GameDataContainer CreateValidContainerWithExposedNTuples( out int affectedCellsCount )
    {
        List<int> sequence = new() {
                 1, 2, 3,  4, 5, 6,  7, 8, 9,
                 4, 5, 6, -3,-3,-3,  0, 0, 0,
                 0, 0, 0,  0, 0, 0,  0, 0, 0,

                 0, 0, 0,  0, 0, 0,  0, 0, 0,
                 0, 0, 0,  0, 0, 0,  0, 0, 0,
                 0, 0, 0,  0, 0, 0,  0, 0, 0,

                 0, 0, 0,  0, 0, 0,  0, 0, 0,
                 0, 0, 0,  0, 0, 0,  0, 0, 0,
                 0, 0, 0,  0, 0, 0,  0, 0, 0,
            };
        affectedCellsCount = GetTotalOfNegativeNumbersInSequence( sequence );

        return CreateGameDataWithSpecificSequence( sequence );
    }

    public static GameDataContainer InitInvalidContainerWithValueAppearingMoreThanOnceSingularCells()
    {
        List<int> sequence = new() {
                1,2,4,4,5,6,7,8,9,
                4,5,6,7,8,9,1,2,3,
                7,8,9,1,2,3,4,5,6,
                2,3,4,5,6,7,8,9,1,
                5,6,7,8,0,1,2,3,4,
                8,9,1,2,3,9,5,6,7,
                3,4,5,6,7,8,9,1,2,
                6,2,8,9,1,2,3,4,5,
                9,1,2,3,4,5,6,7,7
            };
        return CreateGameDataWithSpecificSequence( sequence );
    }

    public static GameDataContainer CreateInvalidContainerWithWrongAmountOfTuples()
    {
        List<int> sequence = new() {
            1,2,3,4,5,6,7,8,9,
            4,5,6,9,8,7,2,1,3,
            8,7,9,3,2,1,6,5,4,
            3,9,8,7,1,5,4,2,6,
            6,1,5,8,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,
        };

        return CreateGameDataWithSpecificSequence( sequence );
    }

    public static GameDataContainer CreateInvalidContainerWithTwoHiddenSingles()
    {
        GameDataContainer gameData = CreateValidContainerWithEmptyCells();

        for ( int i = 3; i < 9; i++ ) {
            gameData.ByRows[ 0 ][ 0 ].RemoveFromCandidates( i );
        }

        for ( int i = 1; i < 9; i++ ) {
            gameData.ByRows[ 0 ][ i ].RemoveFromCandidates( 1 );
            gameData.ByRows[ 0 ][ i ].RemoveFromCandidates( 2 );
        }

        return gameData;
    }

    public static IGeneratorJournalingService GetJournalMock()
    {
        var mock = new Mock<IGeneratorJournalingService>();
        mock.Setup( x =>
            x.AddSubTransaction( It.IsAny<GeneratorTransactionTypes>(), It.IsAny<GameGridCell>(), It.IsAny<int>() )
        );
        return mock.Object;
    }

    public static ISettingsService GetSettingsMock()
    {
        var mock = new Mock<ISettingsService>();

        mock.Setup( x => x.SetAccentColor( It.IsAny<AccentColors>() ) );
        mock.Setup( x => x.GetAccentColor() ).Returns( Colors.DarkMagenta );
        mock.Setup( x => x.SetHighlightingMistakes( It.IsAny<bool>() ) );
        mock.Setup( x => x.SetHighlightingRelatedCells( It.IsAny<bool>() ) );
        mock.Setup( x => x.CanHighlightMistakes() ).Returns(true);
        mock.Setup( x => x.CanHighlightRelatedCells() ).Returns( true );

        return mock.Object;
    }

    private static int CountZeroesInSequence( IEnumerable<int> sequence )
        => sequence.Where( number => number == 0 ).Count();

    private static int GetTotalOfNegativeNumbersInSequence( IEnumerable<int> sequence )
        => -sequence.Where( number => number < 0 ).Sum();

    private static IEnumerable<GameGridCell> GetCellsWithOneCandidate( GameGridCollection collection )
    {
        IEnumerable<GameGridCell> uninitiatedCells = collection.GetCellsWithNoValue();

        if ( !uninitiatedCells.Any() ) {
            return uninitiatedCells;
        }

        IEnumerable<GameGridCell> result = uninitiatedCells.Where( cell => cell.CandidatesCount == 1 );

        return result;
    }
}