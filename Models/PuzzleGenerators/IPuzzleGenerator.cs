namespace YASudoku.Models.PuzzleGenerators;

public interface IPuzzleGenerator
{
    GameDataContainer GenerateNewPuzzle();
}
