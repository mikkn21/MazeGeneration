using MazeGen.Algorithms.actionrecord;
using MazeGen.maze;
using MazeGen.maze.tile;
namespace MazeGen.Algorithms {
    public class Backtracking : IGenerator {

        private Maze _maze;
        private Stack<Tile> _stack;
        private Stack<ActionRecord> _undoStack;
        private Stack<ActionRecord> _redoStack;

        private Random _rand;

        private bool _hasStarted = false;
        private Tile _startTile;

        public bool IsComplete { get; private set; }

        public bool CanUndo => _undoStack.Count > 0; 

        public Backtracking(Maze maze){
            _maze = maze;
            _stack = new Stack<Tile>();
            _undoStack = new Stack<ActionRecord>();
            _redoStack = new Stack<ActionRecord>();
            _rand = new Random(Environment.TickCount);
            IsComplete = false;
            _startTile = SelectRandomTile();   
        }

        public void Step(){
            if (!_hasStarted) {
                _maze.MarkTile(_startTile);
                _stack.Push(_startTile);
                _undoStack.Push(new ActionRecord(_startTile));
                _redoStack.Clear();
                _hasStarted = true;
                return;
            }

            if (_redoStack.Count > 0 ) {
                ActionRecord redoRecord = _redoStack.Pop();
                if (redoRecord.ActionType == MazeActionType.Forward) {
                    _maze.RemoveWallBetween(redoRecord.FromTile!, redoRecord.ToTile!);
                    _maze.MarkTile(redoRecord.ToTile!);
                    _stack.Push(redoRecord.ToTile!);
                }
                else {
                    if (_stack.Count > 0 && _stack.Peek() == redoRecord.PoppedTile) {
                        _stack.Pop();
                    }
                    _maze.MarkTile(redoRecord.PoppedTile!);
                }
                _undoStack.Push(redoRecord);
                if (_stack.Count == 0 && _redoStack.Count == 0) {
                    IsComplete = true;
                }
                return;
            }
 
            if (_stack.Count > 0) {
                Tile currentTile = _stack.Peek();

                // Get a list of unvisited neighbouring tiles in random order
                List<(Tile tile, char)> neighbours = _maze.GetNeighbours(currentTile)
                    .Where(n => n.Item1.State == TileState.Unvisited)
                    .OrderBy(n => _rand.Next())
                    .ToList();

                // Visit neighbour
                if (neighbours.Count > 0) {
                    Tile neighbour = neighbours[0].tile;
                    
                    _maze.RemoveWallBetween(currentTile, neighbour);
                    _maze.MarkTile(neighbour);
                    _undoStack.Push(new ActionRecord(currentTile, neighbour));
                    _stack.Push(neighbour);
                }
                // Backtrack
                else {
                    Tile current_tile = _stack.Pop();
                    _maze.MarkTile(current_tile);
                    _undoStack.Push(new ActionRecord(current_tile));
                }
                _redoStack.Clear();
            }

            if (_stack.Count == 0 ) {
                // Made this such that the ALG is complete at when the final tile becomes white (seleted).
                Console.WriteLine("ALG is complete");
                IsComplete = true;
            }
        }


        // Restart the maze generation 
        // I.e., select new seed and start tile
        public void Restart() {
            _stack.Clear();
            _undoStack.Clear();
            _redoStack.Clear();
            _maze.ResetMaze();
            _rand = new Random(Environment.TickCount);
            _startTile = SelectRandomTile(); 
            _hasStarted = false;
            IsComplete = false;
        }

        public void Back() {
            if (_undoStack.Count == 0 ) {
                return; 
            }

            ActionRecord lastAction = _undoStack.Pop();

            if (lastAction.ActionType == MazeActionType.Forward) {
                
                // Remove the neighbor from the stack if it’s on top.
                if (_stack.Count > 0 && _stack.Peek() == lastAction.ToTile) {
                    _stack.Pop();
                } 
                _maze.SetWallBetween(lastAction.FromTile!, lastAction.ToTile!);
                _maze.UnmarkTile(lastAction.ToTile!);
            }
            else if (lastAction.ActionType == MazeActionType.Backtrack) {
                _stack.Push(lastAction.PoppedTile!);
                // If the stack is empty, we are back to the starting tile.
                // effectively resetting
                if (_undoStack.Count == 0) { 
                    _hasStarted = false;
                }
                _maze.UnmarkTile(lastAction.PoppedTile!);     
            }

            _redoStack.Push(lastAction);
            IsComplete = false;
    
        }


        public Tile SelectRandomTile() {
            int x = _rand.Next(_maze.Width);
            int y = _rand.Next(_maze.Height);
            return _maze.GetTile(x, y);
         
        }




    } // class Backtracking
} // namespace MazeGen.Algorithms
