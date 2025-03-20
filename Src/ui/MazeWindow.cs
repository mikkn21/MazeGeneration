using MazeGen.maze.tile;
using MazeGen.maze.wall;
using Raylib_cs;
using System.Numerics;
using MazeGen.ui.components;
using MazeGen.maze;

namespace MazeGen.ui
{
    public class MazeWindow {

        public int Height { get; private set; }
        public int Width { get; private set; }

        public bool IsComplete => _generator.IsComplete;
        public bool CanUndo => _generator.CanUndo;
        public Tile? CurrentTile => _generator?.CurrentTile;

        public bool IsRunning { get; set;}

        private Maze _maze;

        // _framesPerStep = 1   -> 60 steps per second (fastest)
        // _framesPerStep = 30  -> 2 steps per second (moderate)
        // _framesPerStep = 60  -> 1 step per second (slow)
        // _framesPerStep = 120 -> 0.5 steps per second (very slow)
        private int _framesPerStep;
        private int _framesCounter = 0;
        private readonly int _cellSize; // Size of each cell in pixels
        private readonly int _wallThickness; // Thickness of the walls in pixels
        private IGenerator _generator;

        

        public MazeWindow(Maze maze, int cellSize, IGenerator generator, int wallThickness = 3, int framesPerStep = 1, bool autoRun = false){
            _maze = maze;
            _cellSize = cellSize;
            _wallThickness = wallThickness;
            _framesPerStep = framesPerStep;
            _generator = generator;

            IsRunning = autoRun; 

            Width = _maze.Width * _cellSize;
            Height = _maze.Height * _cellSize; 
        }

        public void restartMaze() {
            _generator.Restart();
        }

        public void Step() {
            _generator.Step();
        }

        public void Back() {
            _generator.Back();
        }


        // offset is an optional paramter that only the preview window is using
        public void DrawFrame(int offsetX = 0, int offsetY = 0) {
            if (!_generator.IsComplete && _framesCounter >= _framesPerStep && IsRunning){
                _generator.Step();
                _framesCounter = 0;
            }
            _framesCounter++;

            Raylib.ClearBackground(Color.White);
            DrawMaze(offsetX, offsetY);
        }

        private void DrawMaze(int offsetX, int offsetY){
            for (int x = 0; x < _maze.Width; x++){
                for (int y = 0; y < _maze.Height; y++){
                    Tile tile = _maze.GetTile(x, y);
                    bool isCurrentTile = _generator.CurrentTile == tile;
                    DrawCell(tile, isCurrentTile, offsetX, offsetY);
                }
            }
        }

        private void DrawCell(Tile tile, bool highlight, int offsetX, int offsetY){
            int posX = tile.X * _cellSize + offsetX;
            int posY = tile.Y * _cellSize + offsetY;
            Raylib.DrawRectangle(posX, posY, _cellSize, _cellSize, tile.Color);

            float adjustOverlap = _wallThickness / 2.0f;

            // To fix overlapping walls between cells:
            // Each cell draws its North and West walls 
            // Esxccept for edges which need to draw the maze boundary 
            if (_maze.HasWall(tile, Wall.North)){
                Vector2 v1 = new Vector2(posX, posY + adjustOverlap);
                Vector2 v2 = new Vector2(posX + _cellSize, posY + adjustOverlap);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }
            if (_maze.HasWall(tile, Wall.West)){
                Vector2 v1 = new Vector2(posX + adjustOverlap, posY);
                Vector2 v2 = new Vector2(posX + adjustOverlap, posY + _cellSize);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }

            // Draw South Wall only if this is the bottom row
            if (tile.Y == _maze.Height - 1 && _maze.HasWall(tile, Wall.South)){
                Vector2 v1 = new Vector2(posX, posY + _cellSize - adjustOverlap);
                Vector2 v2 = new Vector2(posX + _cellSize, posY + _cellSize - adjustOverlap);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }

            // Draw East Wall only if this is the rightmost column
            if (tile.X == _maze.Width - 1 && _maze.HasWall(tile, Wall.East)){
                Vector2 v1 = new Vector2(posX + _cellSize - adjustOverlap, posY);
                Vector2 v2 = new Vector2(posX + _cellSize - adjustOverlap, posY + _cellSize);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }

            if (highlight){
                int centerX = posX + _cellSize / 2;
                int centerY = posY + _cellSize / 2;
                int radius = _cellSize / 4;
                Raylib.DrawCircle(centerX, centerY, radius, Color.Red); 
            }

        }
    }
} 