using MazeGen.maze;
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
            Console.WriteLine("Generating maze using Backtracking algorithm");
            Maze mazeCopy = maze.Copy();
            BuildMaze(0, 0, mazeCopy);
            return mazeCopy;    
        }


        private void BuildMaze(int cx, int cy, Maze maze) {
            if (maze.HasVisited(cx, cy)) {
                return;
            } 
            maze.MarkCell(cx, cy);

            // Get unvisited neighbour cells in random order (LINQ)
            List<(Wall, char)> neighbours = maze.GetNeighbours(cx, cy)
                .Where( cell => (cell.Item1 & Wall.visited) != Wall.visited)
                // .OrderBy (cell => Guid.NewGuid())
                .ToList();


            foreach ((Wall,char) cell in neighbours) {
                int nx = cx + DX[cell.Item2];
                int ny = cy + DY[cell.Item2];
                maze.RemoveWallBetween(cx, cy, nx, ny);
                Console.WriteLine("Removing wall between ({0},{1}) and ({2},{3})", cx, cy, nx, ny);
                BuildMaze(nx, ny, maze);
            }

        }


        
    } // class Backtracking
} // namespace MazeGen.Algorithms
