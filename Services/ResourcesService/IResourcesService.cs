namespace YASudoku.Services.ResourcesService;

public interface IResourcesService
{
    bool TryGetColorByName( string name, out Color color );
}
