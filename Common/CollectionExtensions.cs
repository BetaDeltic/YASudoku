namespace YASudoku.Common;

public static class CollectionExtensions
{
    public static void ForEach<T>( this ICollection<T> collection, Action<T> action )
    {
        foreach ( T item in collection ) action( item );
    }
}
