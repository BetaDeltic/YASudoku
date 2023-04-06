using YASudoku.Models;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public class GameGridVisualDataCollection
{
    public int Count => cells.Count;
    public GameGridCellVisualData this[ int index ] => cells[ index ];

    private readonly List<GameGridCellVisualData> cells;

    private readonly ISettingsService settings;

    public GameGridVisualDataCollection( GameGridCollection allCells, int gridSize, ISettingsService settings )
    {
        if ( allCells.Count == 0 ) throw new ArgumentException( $"Tried to initialize {nameof( GameGridVisualDataCollection )} with empty collection" );

        int expectedCount = gridSize * gridSize;
        if ( allCells.Count != expectedCount ) throw new ArgumentException( $"Tried to initialize {nameof( GameGridVisualDataCollection )} with {allCells.Count} amount of cells, expected {expectedCount}." );

        this.settings = settings;

        cells = new( allCells.Count );

        InitializeCellData( allCells, gridSize );
    }

    private void InitializeCellData( GameGridCollection allCells, int candidateCount )
        => allCells.ForEach( cell => cells.Add( new( settings, candidateCount, cell.CellID, cell.IsLockedForChanges,
            cell.UserFacingValue, cell.CorrectValue ) ) );

    public void ForEach( Action<GameGridCellVisualData> action ) => cells.ForEach( action );

    public IEnumerable<GameGridCellVisualData> Where( Func<GameGridCellVisualData, bool> func ) => cells.Where( func );

    public void ReplaceCollection( GameGridCollection newCells )
    {
        if ( newCells.Count != cells.Count ) return;

        newCells.ForEach( newCell => {
            var matchingCell = cells.First( oldCell => oldCell.CellID == newCell.CellID );
            matchingCell.ResetCellData( newCell.IsLockedForChanges, newCell.UserFacingValue, newCell.CorrectValue );
        } );
    }

    public void RestartCellValues()
    {
        cells.ForEach( cell => {
            cell.RestartValue();
            cell.HideAllCandidates();
        } );
    }
}
