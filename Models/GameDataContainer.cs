using Serilog;
using System.Diagnostics;
using YASudoku.Services.JournalingServices;

namespace YASudoku.Models;

public class GameDataContainer
{
    public readonly GameGridCollection AllCells;
    public readonly List<GameGridCollection> ByRows;
    public readonly List<GameGridCollection> ByColumns;
    public readonly List<GameGridCollection> ByBlocks;

    private readonly int gridSize;
    private readonly int blockSize;

    private readonly IGeneratorJournalingService journalService;

    public GameDataContainer( int gridSize, IGeneratorJournalingService journalService )
    {
        this.gridSize = gridSize;
        this.journalService = journalService;

        blockSize = gridSize switch {
            9 => 3,
            16 => 4,
            _ => throw new ArgumentException( $"Supplied {nameof( gridSize )}({gridSize}) doesn't conform to traditional game constraints. (9x9 or 16x16)" ),
        };

        AllCells = new( gridSize * gridSize );
        ByRows = new( gridSize );
        ByColumns = new( gridSize );
        ByBlocks = new( gridSize );

        for ( int i = 0; i < gridSize; i++ ) {
            ByRows.Add( new( gridSize ) );
            ByColumns.Add( new( gridSize ) );
            ByBlocks.Add( new( gridSize ) );
        }
    }

    public void InitializeCellCollections(
        Action<GameGridCell, int>? cellInitHandler = null,
        Action<GameGridCell, int>? cellRemovedCandidateHandler = null )
    {
        for ( int row = 0; row < gridSize; row++ ) {
            for ( int column = 0; column < gridSize; column++ ) {
                int cellID = row * 9 + column;
                GameGridCell cell = new( gridSize, cellID );

                if ( cellInitHandler != null ) {
                    cell.CellInitialized += cellInitHandler;
                }

                if ( cellRemovedCandidateHandler != null ) {
                    cell.CellCandidateRemoved += cellRemovedCandidateHandler;
                }

                AllCells.Add( cell );
                ByRows[ row ].Add( cell );
                ByColumns[ column ].Add( cell );

                int blockIndex = GetBlockIndex( column, row );
                ByBlocks[ blockIndex ].Add( cell );
            }
        }

        ByRows.ForEach( row => row.CreateLinksForCellsInsideCollection() );
        ByColumns.ForEach( column => column.CreateLinksForCellsInsideCollection() );
        ByBlocks.ForEach( block => block.CreateLinksForCellsInsideCollection() );
    }

    public void ResetContainer() => AllCells.ResetCollection();

    public void DebugPrintGeneratedPuzzle( string annotation = "" )
    {
        if ( !string.IsNullOrEmpty( annotation ) ) {
            Log.Debug( annotation );
            Debug.WriteLine( annotation );
        }

        ByRows.ForEach( row => {
            string rowString = string.Join( ";", row.GetAllPrintableValues() );
            Log.Debug( rowString );
            Debug.WriteLine( rowString );
        } );
    }

    private int GetBlockIndex( int column, int row )
    {
        int rowFloored = row / blockSize;
        int columnFloored = column / blockSize;
        int blockIndex = rowFloored * blockSize + columnFloored;
        return blockIndex;
    }
}
