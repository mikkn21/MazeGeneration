using MazeGen.maze;
using MazeGen.maze.tile;
using Raylib_cs;
namespace MazeGen.Algorithms{
    public class Backtracking{

        private static readonly Dictionary<char, int> DX = new(){
            {'N', 0},
            {'E', 1},
            {'S', 0},
            {'W', -1}
        };
        private static readonly Dictionary<char, int> DY = new(){
            {'N', -1},
            {'E', 0},
            {'S', 1},
            {'W', 0}
        };
    
        public Maze GenerateMaze(Maze maze){
            Maze mazeCopy = maze.Copy();
            BuildMaze(0, 0, mazeCopy);
            return mazeCopy;
        }


        private void BuildMaze(int cx, int cy, Maze maze) {
            if (maze.HasVisited(cx, cy)) { 
                return;
            } 
            maze.MarkTile(cx, cy);

            // Get unvisited neighbour cells in random order (LINQ)
            List<(Tile tiles, char)> neighbours = maze.GetNeighbours(cx, cy)
                .Where(tile => !tile.Item1.Visited)
                .OrderBy(tile => Guid.NewGuid())
                .ToList();


            foreach ((Tile, char) tile in neighbours) {
                int nx = cx + DX[tile.Item2];
                int ny = cy + DY[tile.Item2];
                if (!maze.HasVisited(nx, ny)) { 
                    maze.RemoveWallBetween(cx, cy, nx, ny); 
                    BuildMaze(nx, ny, maze);
                }
            }
        }


        
    } // class Backtracking
} // namespace MazeGen.Algorithms
