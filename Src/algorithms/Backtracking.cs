using MazeGen.maze;
using MazeGen.maze.tile;
namespace MazeGen.Algorithms {
    public class Backtracking : IGenerator {

        private Maze _maze;
        private Stack<Tile> _stack; 
        private Random _rand;
        private Tile _startTile;

        private int _seed;

        public bool IsComplete { get; private set; }

        public Backtracking(Maze maze){
            _maze = maze;
            _stack = new Stack<Tile>();
            _seed = Environment.TickCount;
            _rand = new Random(_seed);
            IsComplete = false;

            // Start at the top left corner
            int x = _rand.Next(_maze.Width); 
            int y = _rand.Next(_maze.Height);
            _startTile = _maze.GetTile(x, y);
            _maze.MarkTile(_startTile);
            _stack.Push(_startTile);
        }


        public void Step(){
            if (_stack.Count > 0) {
                Tile currentTile = _stack.Peek();
                currentTile.Color = Raylib_cs.Color.LightGray; 

                // Get a list of unvisited neighbouring cells
                List<(Tile tile, char)> neighbours = _maze.GetNeighbours(currentTile)
                    .Where(n => !n.Item1.Visited)
                    .OrderBy(n => _rand.Next())
                    .ToList();


                // Visit a random neighbour
                if (neighbours.Count > 0) {
                    Tile neighbour = neighbours[0].tile;

                    _maze.RemoveWallBetween(currentTile, neighbour);
                    _maze.MarkTile(neighbour);
                    _stack.Push(neighbour);
                }
                // Backtrack
                else {
                    Tile current_tile = _stack.Pop();
                    current_tile.Color = Raylib_cs.Color.White;
                }
            }
            else{
                IsComplete = true;
            }
        }

        public void Reset() {
            _stack.Clear();
            _maze.ResetMaze();

            _rand = new Random(_seed);
            int x = _rand.Next(_maze.Width);
            int y = _rand.Next(_maze.Height);
            _startTile = _maze.GetTile(x, y);
            
            _maze.MarkTile(_startTile);
            _stack.Push(_startTile);
            IsComplete = false;
        }

        public void Back()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    } // class Backtracking
} // namespace MazeGen.Algorithms
