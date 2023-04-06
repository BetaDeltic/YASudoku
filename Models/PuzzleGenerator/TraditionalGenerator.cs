using YASudoku.Common;
using YASudoku.Services.JournalingServices;

namespace YASudoku.Models.PuzzleGenerator;

public class TraditionalGenerator : IPuzzleGenerator
{
    private const int gridSize = 9;

    private readonly IGeneratorJournalingService commandJournal;
    private readonly Random random = new();

    private bool isValid = true;
    private CancellationTokenSource cancelSource = new();

    private GameDataContainer? gameData;

    public TraditionalGenerator( IGeneratorJournalingService commandJournal )
    {
        this.commandJournal = commandJournal;
    }

    public GameDataContainer GenerateNewPuzzle()
    {
        gameData = new GameDataContainer( gridSize, commandJournal );
        PuzzleValidator.PuzzleValidator validator = new();
        PuzzleResolver.PuzzleResolver puzzleResolver = new( cancelSource.Token );
        gameData.InitializeCellCollections(
            cellInitHandler: _ => CellActionHandler( validator ),
            cellRemovedCandidateHandler: _ => CellActionHandler( validator ) );

        while ( !FillCellsWithNumbers( puzzleResolver ) ) {
            gameData.DebugPrintGeneratedPuzzle( "Failed to fill cells with numbers" );
            gameData.ResetContainer();
            commandJournal.ClearJournal();
        }

        SetCorrectValuesToFilledNumbers();

        RemoveNumbersForViablePuzzle( puzzleResolver );

        commandJournal.ClearJournal();

        return gameData;
    }

    private void CellActionHandler( PuzzleValidator.PuzzleValidator validator )
    {
        isValid = validator.IsValid( gameData! );
        if ( !isValid ) {
            cancelSource.Cancel();
        }
    }

    private bool FillCellsWithNumbers( PuzzleResolver.PuzzleResolver puzzleResolver )
    {
        foreach ( GameGridCell cell in gameData!.AllCells ) {
            if ( cell.IsInitialized ) continue;

            bool isFilled = false;
            bool transactionReversed;
            do {
                if ( cancelSource.IsCancellationRequested ) {
                    cancelSource = new();
                    puzzleResolver.ResetCancellationToken( cancelSource.Token );
                }
                transactionReversed = false;
                commandJournal.StartTransaction();
                cell.InitializeWithRandomCandidate( random );
                if ( !isValid ) {
                    if ( cell.CandidatesCount == 1 ) {
                        return false;
                    }
                    commandJournal.ReverseTransaction();
                    transactionReversed = true;
                    continue;
                }

                isFilled = puzzleResolver.TryResolve( gameData );

                if ( isValid ) continue;

                commandJournal.ReverseTransaction();
                transactionReversed = true;
            } while ( transactionReversed );

            commandJournal.CommitTransaction();
            gameData.DebugPrintGeneratedPuzzle( $"Generated so far:" );

            if ( isFilled ) {
                break;
            }
        }
        return true;
    }

    private void SetCorrectValuesToFilledNumbers()
        => gameData?.AllCells.ForEach( cell => cell.SetCorrectValueToUserFacingValue() );

    private void RemoveNumbersForViablePuzzle( PuzzleResolver.PuzzleResolver puzzleResolver )
    {
        int cellsCount = gameData!.AllCells.Count;
        List<int> untouchedCellIndexes = new( Enumerable.Range( 0, cellsCount - 1 ) );
        List<int> removedNumberCellIndexes = new( cellsCount );
        while ( untouchedCellIndexes.Count > 0 ) {
            RemoveValuesFromCellsOnList( removedNumberCellIndexes );

            int untouchedIndex = random.Next( untouchedCellIndexes.Count );
            int cellIndexToDelete = untouchedCellIndexes[ untouchedIndex ];
            GameGridCell deletedValueCell = gameData.AllCells[ cellIndexToDelete ];
            deletedValueCell.RemoveUserFacingValue();
            deletedValueCell.RecalculateCandidates();

            RecalculateCandidatesForRemovedNumbers( removedNumberCellIndexes );

            if ( puzzleResolver.TryResolve( gameData ) ) {
                CheckSolutionValidity( removedNumberCellIndexes );
                removedNumberCellIndexes.Add( cellIndexToDelete );
            } else {
                deletedValueCell.ResetUserFacingValueToCorrectValue();
            }

            if ( !untouchedCellIndexes.Remove( cellIndexToDelete ) ) {
                throw new ApplicationException( "Trying to delete wrong index." );
            }
        }

        if ( !removedNumberCellIndexes.Any() ) {
            throw new ApplicationException( "Didn't delete anything" );
        }

        RemoveValuesFromCellsOnList( removedNumberCellIndexes );
        RecalculateCandidatesForRemovedNumbers( removedNumberCellIndexes );
        if ( !puzzleResolver.TryResolve( gameData ) ) {
            throw new ApplicationException( "Resolver is unable to solve it on second attempt." );
        }
        CheckSolutionValidity( removedNumberCellIndexes );

        RemoveValuesFromCellsOnList( removedNumberCellIndexes );

        LockFilledCellsForChanges();
    }

    private void RemoveValuesFromCellsOnList( List<int> removedNumberCellIndexes )
    {
        removedNumberCellIndexes.ForEach( index => {
            gameData!.AllCells[ index ].RemoveUserFacingValue();
        } );
    }

    private void RecalculateCandidatesForRemovedNumbers( List<int> removedNumberCellIndexes )
    {
        removedNumberCellIndexes.ForEach( index => {
            gameData!.AllCells[ index ].RecalculateCandidates();
        } );
    }

    private void CheckSolutionValidity( List<int> removedNumberCellIndexes )
    {
        removedNumberCellIndexes.ForEach( index => {
            GameGridCell currentCell = gameData!.AllCells[ index ];
            if ( !currentCell.HasCorrectValue ) {
                throw new ApplicationException( $"Resolver did bad, cell #{currentCell.CellID}, resolver value: {currentCell.UserFacingValue}" );
            }
        } );
        gameData!.AllCells.ForEach( cell => {
            if ( !cell.HasUserFacingValue ) {
                throw new ApplicationException( "Resolver didn't resolve everything." );
            }
        } );
    }

    private void LockFilledCellsForChanges()
        => gameData!.AllCells
        .Where( cell => cell.HasUserFacingValue )
        .ForEach( cell => cell.LockForUserChanges() );
}
