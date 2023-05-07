namespace YASudoku.Services.ResourcesService;

public class ResourcesService : IResourcesService
{
    public bool TryGetColorByName( string name, out Color color )
    {
        color = Colors.Transparent;
        if ( Application.Current == null ) return false;

        bool success = Application.Current.Resources.TryGetValue( name, out object colorObject );
        if ( !success || colorObject is not Color ) return false;

        color = (Color)colorObject;

        return true;
    }
}
