using MazeGen.maze.tile;
using MazeGen.maze.wall;

namespace MazeGen.maze {

    public class Maze {
        
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Tile[,] tiles;

        public Maze(int width, int height) {

            Width = width;
            Height = height;
            tiles = new Tile[width, height];
        
            // init maze with all walls 
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    tiles[x, y] = new Tile(x, y); 
                }
            }
        }

        // Mark a cell as visisted 
        public void MarkTile(Tile tile) {
            if (!HasVisited(tile)) {
                tile.Visited = true;
            }
        }

        // Check if a cell has been visited
        public bool HasVisited(Tile tile) {
            return tile.Visited;
        }

        // Get the tile at (x, y)
        public Tile GetTile(int x, int y) {
            return tiles[x, y];
        }

        // Get the neighbours of tile 
        public List<(Tile, char)> GetNeighbours(Tile tile) {
            int x = tile.X;
            int y = tile.Y;
            
            List<(Tile, char)> neighbours = [];
            // North
            if (InBounds(x, y-1)) {
                neighbours.Add((tiles[x, y-1], 'N'));
            }
            // South
            if (InBounds(x, y+1)) { 
                neighbours.Add((tiles[x, y+1], 'S'));
            }
            // East
            if (InBounds(x+1, y)) {
                neighbours.Add((tiles[x+1, y], 'E'));
            }
            // West 
            if (InBounds(x-1, y)) {
                neighbours.Add((tiles[x-1, y], 'W'));   
            }
            return neighbours;
        }
        
        ///  Remove the wall between two adjacent cells at (x1, y1) and (x2, y2)
        public void RemoveWallBetween(Tile tile1, Tile tile2) {
            int x1 = tile1.X;
            int y1 = tile1.Y;
            int x2 = tile2.X;
            int y2 = tile2.Y;

            if (!(InBounds(x1, y1) && InBounds(x2, y2))) {
                throw new ArgumentException("Invalid coordinates");
            }

            // Check if (x2, y2) is North of (x1, y1)
            if (x1 == x2 && y2 == y1 - 1) { 
                tiles[x1, y1].Walls &= ~Wall.North;
                tiles[x2, y2].Walls  &= ~Wall.South;
            }
            // Check if (x2, y2) is South of (x1, y1)
            else if (x1 == x2 && y2 == y1 + 1) {
                tiles[x1, y1].Walls &= ~Wall.South;
                tiles[x2, y2].Walls &= ~Wall.North;
            }
            // Check if (x2, y2) is East of (x1, y1)
            else if (y1 == y2 && x2 == x1 + 1) {
                tiles[x1, y1].Walls &= ~Wall.East;
                tiles[x2, y2].Walls &= ~Wall.West;
            }
            // Check if (x2, y2) is West of (x1, y1)
            else if (y1 == y2 && x2 == x1 - 1) {
                tiles[x1, y1].Walls &= ~Wall.West;
                tiles[x2, y2].Walls &= ~Wall.East;
            } else {
                throw new ArgumentException("Cells are not adjacent");
            }

        }

        /// Check if a wall is present at a given cell (x, y)
        public bool HasWall(int x, int y, Wall wall) {
            if (!InBounds(x, y)) {
               return true; 
            } 
            return (tiles[x, y].Walls & wall) != 0;
        }


        ///  Check if a cell is within the bounds of the maze
        public bool InBounds(int x, int y) {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        } 

        ///  Copy the maze
        public Maze Copy(){
            Maze clone = new Maze(Width, Height);

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Tile original = this.tiles[x, y];
                    clone.tiles[x, y] = new Tile(original.X, original.Y) {
                        Walls = original.Walls,
                        Color = original.Color,
                        Visited = original.Visited
                    };
                }
            }
            return clone;
        }



    } 
} 