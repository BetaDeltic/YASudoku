namespace YASudoku.Common;

public static class IEnumerableExtensions
{
    public static void ForEach<T>( this IEnumerable<T> collection, Action<T> action )
    {
        foreach ( T item in collection ) action( item );
    }

    public static bool HasDistinctValues<T>( this IEnumerable<T> collection )
    {
        HashSet<T> distinctValues = new();

        return collection.All( value => distinctValues.Add( value ) );
    }
}
