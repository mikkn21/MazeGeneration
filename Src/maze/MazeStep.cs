using Raylib_cs;

namespace MazeGen.maze.step {
    public class MazeStep
    {
        public int Y { get; set; }
        public int X { get; set; }
        public required string Action { get; set; }  // "visit", "removeWall", etc.
        public int NeighborX { get; set; }
        public int NeighborY { get; set; }
        public Color color { get; set; }
    }

}