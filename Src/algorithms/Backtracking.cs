using MazeGen.maze;
namespace MazeGen.Algorithms{
    public class Backtracking{

        // constant to help with directions
        private const int NORTH = 1;
        private const int EAST = 2;
        private const int SOUTH = 4;
        private const int WEST = 8;

        private static readonly Dictionary<int, int> DX = new(){
            {NORTH, 0},
            {EAST, 1},
            {SOUTH, 0},
            {WEST, -1}
        };
        private static readonly Dictionary<int, int> DY = new(){
            {NORTH, -1},
            {EAST, 0},
            {SOUTH, 1},
            {WEST, 0}
        };

        private static readonly Dictionary<int, int> OPPOSITE = new(){
            {NORTH, SOUTH},
            {EAST, WEST},
            {SOUTH, NORTH},
            {WEST, EAST}
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
            maze.VisitCell(cx, cy);

            // LINQ
            int[] directions = new int[] { NORTH, EAST, SOUTH, WEST }
                .OrderBy(x => Guid.NewGuid())
                .ToArray();
            

            foreach (int dir in directions) {
                int nx = cx + DX[dir];
                int ny = cy + DY[dir];
 
                if (maze.HasWall(nx, ny, (Wall)dir)) {
                    try {
                        maze.RemoveWallBetween(cx, cy, nx, ny);
                    } catch (ArgumentException) {
                        continue; // Skip as we hit the border of the maze
                    }
                
                    BuildMaze(nx, ny, maze);
                }
            }
        }


        
    } // class Backtracking
} // namespace MazeGen.Algorithms
