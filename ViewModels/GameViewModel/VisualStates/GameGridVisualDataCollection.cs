using YASudoku.Models;
using YASudoku.Services.JournalingServices;
using YASudoku.Services.SettingsService;

namespace YASudoku.ViewModels.GameViewModel.VisualStates;

public class GameGridVisualDataCollection
{
    public int Count => visualCells.Count;
    public GameGridCellVisualData this[ int index ] => visualCells[ index ];

    private readonly IServiceProvider serviceProvider;

    private readonly List<GameGridCellVisualData> visualCells;

    public GameGridVisualDataCollection( GameGridCollection generatedCellData, int gridSize,
        IServiceProvider serviceProvider )
    {
        this.serviceProvider = serviceProvider;

        if ( generatedCellData.Count == 0 )
            throw new ArgumentException( $"Tried to initialize {nameof( GameGridVisualDataCollection )} with empty collection" );

        int expectedCount = gridSize * gridSize;
        if ( generatedCellData.Count != expectedCount )
            throw new ArgumentException( $"Tried to initialize {nameof( GameGridVisualDataCollection )} with {generatedCellData.Count} amount of cells, expected {expectedCount}." );

        visualCells = new( generatedCellData.Count );

        InitializeCellData( generatedCellData, gridSize );
    }

    internal GameGridVisualDataCollection( GameGridVisualDataCollection collection, IServiceProvider serviceProvider )
    {
        this.serviceProvider = serviceProvider;

        visualCells = new( collection.Count );
        collection.ForEach( cell => visualCells.Add( cell ) );
    }

    private void InitializeCellData( GameGridCollection generatedCellData, int candidateCount )
    {
        ISettingsService settings = serviceProvider.GetRequiredService<ISettingsService>();
        IPlayerJournalingService journal = serviceProvider.GetRequiredService<IPlayerJournalingService>();

        generatedCellData.ForEach( generatedCell => {
            visualCells.Add( new( settings, journal, candidateCount, generatedCell.CellID, generatedCell.IsLockedForChanges,
                generatedCell.UserFacingValue, generatedCell.CorrectValue ) );
        } );

        generatedCellData.ForEach( generatedCell => {
            generatedCell.relatedCells.ForEach( relatedCell => {
                visualCells[ generatedCell.CellID ].AddRelatedCell( visualCells[ relatedCell.CellID ] );
            } );
        } );
    }

    public bool Any( Func<GameGridCellVisualData, bool> func ) => visualCells.Any( func );

    public bool All( Func<GameGridCellVisualData, bool> func ) => visualCells.All( func );

    public void ForEach( Action<GameGridCellVisualData> action ) => visualCells.ForEach( action );

    internal IEnumerable<TOutput> Select<TOutput>( Func<GameGridCellVisualData, int, TOutput> func )
        => visualCells.Select( func );

    public IEnumerable<GameGridCellVisualData> Where( Func<GameGridCellVisualData, bool> func )
        => visualCells.Where( func );

    public void ReplaceCollection( GameGridCollection newCells )
    {
        if ( newCells.Count != visualCells.Count ) return;

        newCells.ForEach( newCell => {
            GameGridCellVisualData matchingCell = visualCells[ newCell.CellID ];
            matchingCell.ResetCellData( newCell.IsLockedForChanges, newCell.UserFacingValue, newCell.CorrectValue );
        } );
    }

    public void RestartCellValues()
    {
        visualCells.ForEach( cell => {
            cell.RestartCell();
            cell.RemoveAllCandidates();
        } );
    }
}
