using YASudoku.Common;

namespace YASudoku.Models;

public class GameGridCollection
{
    public int Count => cells.Count;
    public GameGridCell this[ int index ] => cells[ index ];

    private readonly List<GameGridCell> cells;

    public GameGridCollection( int collectionSize )
    {
        cells = new( collectionSize );
    }

    public void Add( GameGridCell cell ) => cells.Add( cell );

    public void CreateLinksForCellsInsideCollection()
    {
        cells.ForEach( cell1 => {
            cells.Where( cell => cell != cell1 ).ForEach( cell2 => {
                cell1.AddRelatedCell( cell2 );
                cell2.AddRelatedCell( cell1 );
            } );
        } );
    }

    public void ForEach( Action<GameGridCell> action ) => cells.ForEach( action );

    public IEnumerable<string> GetAllPrintableValues() => cells.Select( cell => cell.ToString() );

    internal IEnumerable<int> GetAllCellValues()
        => cells.Where( cell => cell.IsInitialized ).Select( cell => cell.UserFacingValue );

    public Dictionary<int, int> GetCandidateCounts()
    {
        IEnumerable<IEnumerable<int>> allCandidates = GetEnumerableOfAllCandidates();

        Dictionary<int, int> candidateCounts = allCandidates
            .SelectMany( candidates => candidates )
            .GroupBy( candidate => candidate )
            .ToDictionary( group => group.Key, group => group.Count() );

        return candidateCounts;
    }

    public IEnumerable<GameGridCell> GetCellsWithNoValue()
        => GetUninitiatedCells().Any() ? GetUninitiatedCells() : GetCellsWithRemovedValue();

    private IEnumerable<GameGridCell> GetUninitiatedCells()
        => cells.Where( cell => !cell.IsInitialized );

    private IEnumerable<GameGridCell> GetCellsWithRemovedValue()
        => cells.Where( cell => cell.IsInitialized && !cell.HasUserFacingValue );

    // Using a list here, because we need to get the counts of the cells with the specific candidate every time
    public List<GameGridCell> GetCellsWithSpecificCandidate( int candidate )
    {
        List<GameGridCell> cellsWithCandidate = GetCellsWithNoValue()
            .Where( cell => cell.Candidates.Contains( candidate ) ).ToList();

        return cellsWithCandidate;
    }

    public IEnumerable<IEnumerable<int>> GetEnumerableOfAllCandidates()
        => GetCellsWithNoValue().Select( cell => cell.Candidates );

    public List<GameGridCell>.Enumerator GetEnumerator() => cells.GetEnumerator();

    public IEnumerable<GameGridCell> GetInitiatedCells() => cells.Where( cell => cell.IsInitialized );

    public IEnumerable<IEnumerable<GameGridCell>> GetCellsWithNoValueGroupedByNonSingleCandidates()
    {
        return GetCellsWithNoValue()
           .Where( cell => cell.CandidatesCount > 1 )
           .GroupBy(
               keySelector: cell => cell.CandidatesText,
               elementSelector: cell => cell,
               resultSelector: ( _, cell ) => cell
            );
    }

    public bool HasDistinctValues()
    {
        if ( cells.Count == 0 ) {
            throw new ApplicationException( "Inner cells collection is not initialized!" );
        }

        IEnumerable<int> initializedCellValues = cells
            .Where( cell => cell.IsInitialized )
            .Select( cell => cell.UserFacingValue );

        return initializedCellValues.HasDistinctValues();
    }

    public void ResetCollection() => cells.ForEach( cell => cell.ResetCell() );

    public IEnumerable<GameGridCell> Where( Func<GameGridCell, bool> func ) => cells.Where( func );
}
