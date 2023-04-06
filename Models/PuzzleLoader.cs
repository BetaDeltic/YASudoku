using YASudoku.Services.JournalingServices;
using YASudoku.Services.SettingsService;

namespace YASudoku.Models;

public class PuzzleLoader
{
    public GameDataContainer LoadExistingPuzzle( string name )
    {
        int gridSize = 9;
        int amountOfCells = gridSize * gridSize;
        GameDataContainer gameData = new( amountOfCells, new GeneratorJournalingService() );
        for ( int i = 0; i < amountOfCells; i++ ) {
            //gameData.AllCells.Add( new GameGridCell( gridSize, i ) );
        }
        return gameData;
    }
}
