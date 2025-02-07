using MazeGen.maze;
using MazeGen.maze.step;
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

        private List<MazeStep> steps = new();

    
        public List<MazeStep> GenerateMaze(Maze maze){
            Maze mazeCopy = maze.Copy();
            steps.Clear(); 
            BuildMaze(0, 0, mazeCopy);
            return steps;
        }


        private void BuildMaze(int cx, int cy, Maze maze) {
            if (maze.HasVisited(cx, cy)) {
                steps.Add(new MazeStep {
                    X = cx,
                    Y = cy,
                    Action = "visited",
                    color = Color.Blue,
                });
                return;
            } 
            maze.MarkCell(cx, cy);
            steps.Add(new MazeStep {
                X = cx,
                Y = cy,
                Action = "visit",
                color = Color.Red,
            });

            // Get unvisited neighbour cells in random order (LINQ)
            List<(Wall, char)> neighbours = maze.GetNeighbours(cx, cy)
                .Where( cell => (cell.Item1 & Wall.visited) != Wall.visited)
                .OrderBy (cell => Guid.NewGuid())
                .ToList();

         
            foreach ((Wall,char) cell in neighbours) {
                int nx = cx + DX[cell.Item2];
                int ny = cy + DY[cell.Item2];
                if (!maze.HasVisited(nx, ny)) { 
                    maze.RemoveWallBetween(cx, cy, nx, ny);
                    steps.Add(new MazeStep {
                        X = cx,
                        Y = cy,
                        NeighborX = nx,
                        NeighborY = ny,
                        Action = "removeWall",
                        color = Color.Red,
                    });
                    BuildMaze(nx, ny, maze);
                }
            }
        }


        
    } // class Backtracking
} // namespace MazeGen.Algorithms
