using MazeGen.maze.tile;

public interface IGenerator {
    void Step();

    void Restart(); 

    void Back(); 

    Tile? CurrentTile { get; }

    bool IsComplete { get; }

    bool CanUndo { get; }
}