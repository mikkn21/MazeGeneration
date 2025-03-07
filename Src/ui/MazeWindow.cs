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


        public MazeWindow(Maze maze, int cellSize, IGenerator generator, int wallThickness = 3, int framesPerStep = 1){
            _maze = maze;
            _cellSize = cellSize;
            _wallThickness = wallThickness;
            _framesPerStep = framesPerStep;
            _generator = generator;

            Width = _maze.Width * _cellSize;
            Height = _maze.Height * _cellSize; 
        }

        public void restartMaze() {
            _generator.Restart();
        }

        public void DrawFrame() {
            if (!_generator.IsComplete && _framesCounter >= _framesPerStep){
                _generator.Step();
                _framesCounter = 0;
            }
            _framesCounter++;

            Raylib.ClearBackground(Color.White);
            DrawMaze();


            // draw border                
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            Raylib.DrawRectangleLinesEx(rect, _wallThickness, Color.Black);
        }


        private void DrawMaze(){
            for (int x = 0; x < _maze.Width; x++){
                for (int y = 0; y < _maze.Height; y++){
                    Tile tile = _maze.GetTile(x, y);
                    bool isCurrentTile = _generator.currentTile == tile;
                    DrawCell(tile, isCurrentTile);
                }
            }
        }

        private void DrawCell(Tile tile, bool highlight){
            int posX = tile.X * _cellSize;
            int posY = tile.Y * _cellSize;
            Raylib.DrawRectangle(posX, posY, _cellSize, _cellSize, tile.Color);

            // Draw walls
            if (_maze.HasWall(tile, Wall.North)){
                Vector2 v1 = new Vector2(posX, posY);
                Vector2 v2 = new Vector2(posX + _cellSize, posY);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }
            if (_maze.HasWall(tile, Wall.East)){
                Vector2 v1 = new Vector2(posX + _cellSize, posY);
                Vector2 v2 = new Vector2(posX + _cellSize, posY + _cellSize);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }
            if (_maze.HasWall(tile, Wall.South)){
                Vector2 v1 = new Vector2(posX, posY + _cellSize);
                Vector2 v2 = new Vector2(posX + _cellSize, posY + _cellSize);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }
            if (_maze.HasWall(tile, Wall.West)){
                Vector2 v1 = new Vector2(posX, posY);
                Vector2 v2 = new Vector2(posX, posY + _cellSize);
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