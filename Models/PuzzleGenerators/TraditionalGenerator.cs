using YASudoku.Common;
using YASudoku.Models.PuzzleResolvers;
using YASudoku.Models.PuzzleValidators;
using YASudoku.Services.JournalingServices;

namespace YASudoku.Models.PuzzleGenerators;

public class TraditionalGenerator : IPuzzleGenerator
{
    private const int gridSize = 9;

    private readonly IGeneratorJournalingService journal;
    private readonly IPuzzleResolver resolver;
    private readonly IPuzzleValidator validator;
    private readonly Random random = new();

    private bool isValid = true;

    private GameDataContainer? gameData;

    private CancellationTokenSource _cancelSource;

    private CancellationTokenSource CancelSource
    {
        get => _cancelSource;
        set {
            _cancelSource = value;
            resolver.SetCancellationToken( _cancelSource.Token );
        }
    }

    public TraditionalGenerator( IGeneratorJournalingService commandJournal, IPuzzleResolver puzzleResolver, IPuzzleValidator puzzleValidator )
    {
        journal = commandJournal;
        resolver = puzzleResolver;
        validator = puzzleValidator;

        CancelSource = new();

        if ( _cancelSource == null ) throw new NullReferenceException( "Cancel source is null" );
    }

    public GameDataContainer GenerateNewPuzzle()
    {
        InitializeGameDataContainer();
        if ( gameData == null ) throw new ApplicationException( "Game Data Container is null" );

        while ( !FillCellsWithNumbers() ) {
            gameData.DebugPrintGeneratedPuzzle( "Failed to fill cells with numbers" );
            gameData.ResetContainer();
            journal.ClearJournal();
        }

        RemoveNumbersForViablePuzzle();

        return gameData;
    }

    private void InitializeGameDataContainer()
    {
        if ( gameData == null ) {
            gameData = new GameDataContainer( gridSize, journal );
            gameData.InitializeCellCollections( CellInitializedHandler, CellCandidateRemovedHandler );
        } else {
            gameData.ResetContainer();
            journal.ClearJournal();
            CancelSource = new();
        }
    }

    private void CellInitializedHandler( GameGridCell initializedCell, int initValue )
    {
        journal.AddSubTransaction( GeneratorTransactionTypes.CellValueInitialized, initializedCell, initValue );

        RunValidator();
    }

    private void CellCandidateRemovedHandler( GameGridCell affectedCell, int removedCandidate )
    {
        journal.AddSubTransaction( GeneratorTransactionTypes.CandidateRemoved, affectedCell, removedCandidate );

        RunValidator();
    }

    private void RunValidator()
    {
        isValid = validator.IsValid( gameData! );

        if ( !isValid ) CancelSource.Cancel();
    }

    private bool FillCellsWithNumbers()
    {
        foreach ( GameGridCell cell in gameData!.AllCells ) {
            if ( cell.IsInitialized ) continue;

            bool isFilled;
            do {
                if ( CancelSource.IsCancellationRequested ) CancelSource = new();

                journal.StartTransaction();
                cell.InitializeWithRandomCandidate( random );
                if ( !isValid ) {
                    // cell has only one candidate left and it's not valid, meaning - it's not possible to fill the board with current setup
                    if ( cell.CandidatesCount == 1 ) return false;

                    journal.ReverseTransaction();
                    continue;
                }

                isFilled = resolver.TryResolve( gameData );

                if ( isValid ) break;

                if ( cell.CandidatesCount == 1 ) return false;
                journal.ReverseTransaction();
            } while ( true );

            journal.CommitTransaction();
            gameData.DebugPrintGeneratedPuzzle( "Generated so far:" );

            if ( isFilled ) return true;
        }

        // Should never reach here
        return false;
    }

    private void RemoveNumbersForViablePuzzle()
    {
        int cellsCount = gameData!.AllCells.Count;
        List<int> untouchedCellIndexes = new( Enumerable.Range( 0, cellsCount - 1 ) );
        List<int> removedNumberCellIndexes = new( cellsCount );
        while ( untouchedCellIndexes.Count > 0 ) {
            RemoveValuesFromCellsOnList( removedNumberCellIndexes );

            int untouchedIndex = random.Next( untouchedCellIndexes.Count );
            int cellIndexToDelete = untouchedCellIndexes[ untouchedIndex ];
            untouchedCellIndexes.Remove( cellIndexToDelete );

            GameGridCell deletedValueCell = gameData.AllCells[ cellIndexToDelete ];
            deletedValueCell.RemoveUserFacingValue();
            deletedValueCell.RecalculateCandidates();

            RecalculateCandidatesForRemovedNumbers( removedNumberCellIndexes );

            if ( resolver.TryResolve( gameData ) ) {
                removedNumberCellIndexes.Add( cellIndexToDelete );
            } else {
                deletedValueCell.ResetUserFacingValueToCorrectValue();
                deletedValueCell.LockForUserChanges();
            }
        }

        if ( !removedNumberCellIndexes.Any() ) throw new ApplicationException( "Didn't delete anything" );

        RemoveValuesFromCellsOnList( removedNumberCellIndexes );
        RecalculateCandidatesForRemovedNumbers( removedNumberCellIndexes );
    }

    private void RemoveValuesFromCellsOnList( List<int> removedNumberCellIndexes )
        => removedNumberCellIndexes.ForEach( index => gameData!.AllCells[ index ].RemoveUserFacingValue() );

    private void RecalculateCandidatesForRemovedNumbers( List<int> removedNumberCellIndexes )
        => removedNumberCellIndexes.ForEach( index => gameData!.AllCells[ index ].RecalculateCandidates() );
}
