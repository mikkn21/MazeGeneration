using MazeGen.maze.tile;

public interface IGenerator {
    void Step();

    void Restart(); 

    void Back(); 

    Tile? currentTile { get; }

    bool IsComplete { get; }

    bool CanUndo { get; }
}