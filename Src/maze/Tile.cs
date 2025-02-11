using MazeGen.maze.wall;
namespace MazeGen.maze.tile {
    public class Tile {        
        public int X { get; set; }
        public int Y { get; set; }

        public Wall Walls { get; set; }

        public Raylib_cs.Color Color { get; set; }

        public bool Visited { get; set; }

        public Tile(int x, int y) {
            X = x;
            Y = y;
            Walls = Wall.North | Wall.East | Wall.South | Wall.West;
            Color = Raylib_cs.Color.Gray;
            Visited = false;
        }
    }  
} 