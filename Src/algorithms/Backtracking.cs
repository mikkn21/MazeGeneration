using MazeGen.maze;
using MazeGen.maze.tile;
namespace MazeGen.Algorithms {
    public class Backtracking : IGenerator {

        private Maze _maze;
        private Stack<Tile> _stack; 
        private Random _rand;

        public bool IsComplete { get; private set; }

        public Backtracking(Maze maze){
            _maze = maze;
            _stack = new Stack<Tile>();
            _rand = new Random();
            IsComplete = false;

            // Start at the top left corner
            _maze.MarkTile(0, 0);
            _stack.Push(_maze.GetTile(0, 0));
        }


        public void Step(){
            if (_stack.Count > 0)
            {
                Tile currentTile = _stack.Peek();
                int cx = currentTile.X;
                int cy = currentTile.Y;

                // Get a list of unvisited neighbouring cells
                List<(Tile tile, char)> neighbours = _maze.GetNeighbours(cx, cy)
                    .Where(n => !n.Item1.Visited)
                    .OrderBy(n => _rand.Next())
                    .ToList();


                if (neighbours.Count > 0){
                    Tile neighbour = neighbours[0].Item1;
                    int nx = neighbour.X;
                    int ny = neighbour.Y;

                    _maze.RemoveWallBetween(cx, cy, nx, ny);
                    _maze.MarkTile(nx, ny);
                    _stack.Push(neighbour);
                }
                else{
                    _stack.Pop();
                }
            }
            else{
                IsComplete = true;
            }
        }

        // public Maze GenerateMaze(Maze maze)
        // {
        //     Maze mazeCopy = maze.Copy();
        //     BuildMaze(0, 0, mazeCopy);
        //     return mazeCopy;
        // }


        // private void BuildMaze(int cx, int cy, Maze maze) {
        //             if (maze.HasVisited(cx, cy)) { 
        //                 return;
        //             } 
        //             maze.MarkTile(cx, cy);

        //             // Get unvisited neighbour cells in random order (LINQ)
        //             List<(Tile tiles, char)> neighbours = maze.GetNeighbours(cx, cy)
        //                 .Where(tile => !tile.Item1.Visited)
        //                 .OrderBy(tile => Guid.NewGuid())
        //                 .ToList();


        //             foreach ((Tile, char) tile in neighbours) {
        //                 int nx = cx + DX[tile.Item2];
        //                 int ny = cy + DY[tile.Item2];
        //                 if (!maze.HasVisited(nx, ny)) { 
        //                     maze.RemoveWallBetween(cx, cy, nx, ny); 
        //                     BuildMaze(nx, ny, maze);
        //                 }
        //             }
        //         }



    } // class Backtracking
} // namespace MazeGen.Algorithms
