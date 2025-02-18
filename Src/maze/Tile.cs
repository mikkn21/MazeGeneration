using MazeGen.maze.wall;
namespace MazeGen.maze.tile {

    public enum TileState {
        Unvisited,
        Visited,
        Selected
    }


    public class Tile {        
        public int X { get; set; }
        public int Y { get; set; }

        public Wall Walls { get; set; }

        public Raylib_cs.Color Color { get; set; }

        // public bool Visited { get; set; }
        public TileState State { get; set; }

        public Tile(int x, int y) {
            X = x;
            Y = y;
            Walls = Wall.North | Wall.East | Wall.South | Wall.West;
            Color = Raylib_cs.Color.Gray;
            // Visited = false;
            State = TileState.Unvisited;
        }

        public Tile Copy() {
            return new Tile(X, Y) {
                Walls = this.Walls,
                Color = this.Color,
                State = this.State
                // Visited = this.Visited
            };
        }

        public override string ToString() {
           return $"Tile({X}, {Y}) Walls:{Walls} State:{State}";
        }
    }  
} 