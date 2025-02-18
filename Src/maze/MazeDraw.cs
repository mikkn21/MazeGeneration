using MazeGen.ui.button;
using MazeGen.maze.tile;
using MazeGen.maze.wall;
using Raylib_cs;
using System.Numerics;

namespace MazeGen.maze.draw
{
    public class MazeDraw {
        private Maze _maze;

        // _framesPerStep = 1   -> 60 steps per second (fastest)
        // _framesPerStep = 30  -> 2 steps per second (moderate)
        // _framesPerStep = 60  -> 1 step per second (slow)
        // _framesPerStep = 120 -> 0.5 steps per second (very slow)
        private int _framesPerStep;
        private int _framesCounter = 0;
        private readonly int _cellSize; // Size of each cell in pixels
        private readonly int _wallThickness; // Thickness of the walls in pixels
        private readonly int _screenWidth; 
        private readonly int _screenHeight;
        private IGenerator _generator; // TODO: Make array later

        private bool _isRunning = false;

        private Button _backButton;
        private Button _runStopRestartButton;
        private Button _stepButton;
    
        // Button properties
        private const int _buttonWidth = 150;
        private const int _buttonHeight = 40;
        private const int _buttonMargin = 10;

        // TODO: Make generator an array later  
        public MazeDraw(Maze maze, int cellSize, IGenerator generator, int wallThickness = 3, int framesPerStep = 1){
            _maze = maze;
            _cellSize = cellSize;
            _wallThickness = wallThickness;
            _framesPerStep = framesPerStep;
            _generator = generator;

            _screenWidth = _maze.Width * _cellSize;
            _screenHeight = _maze.Height * _cellSize + _buttonHeight + (2*_buttonMargin);
            (_backButton, _runStopRestartButton, _stepButton) = InitButtons();

        }

    
        public void Draw() {
            Raylib.InitWindow(_screenWidth, _screenHeight, "Maze Generator");
            Raylib.SetTargetFPS(60);


            // Main rendering loop
            while (!Raylib.WindowShouldClose()){
                Vector2 mousePos = Raylib.GetMousePosition();


                if (!_generator.IsComplete && _isRunning && _framesCounter >= _framesPerStep){
                    _generator.Step();
                    _framesCounter = 0;
                }
                _framesCounter++;

                if (_generator.IsComplete) {
                    _runStopRestartButton.Label = "Restart";
                    _stepButton.IsEnabled = false;
                }
                else {
                    _stepButton.IsEnabled = true;
                }

                if (_isRunning && !_generator.IsComplete){
                    // If the "run" button is pressed, disable the other buttons
                    _stepButton.IsEnabled = false;
                    _backButton.IsEnabled = false;
                } else {
                    _backButton.IsEnabled = _generator.CanUndo;
                }


                _backButton.Update(mousePos);
                _runStopRestartButton.Update(mousePos);
                _stepButton.Update(mousePos);


                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);

                DrawMaze();


                // draw border                
                Rectangle rect = new Rectangle(0, 0, _screenWidth, _screenHeight);
                Raylib.DrawRectangleLinesEx(rect, _wallThickness, Color.Black);


                _backButton.Draw();
                _runStopRestartButton.Draw();
                _stepButton.Draw();

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }


        private (Button back, Button runReset, Button step) InitButtons() { 
             int horizontalCenterPos = (_screenWidth - _buttonWidth) / 2;

            Button back = new Button(
                new Rectangle(horizontalCenterPos - _buttonWidth - _buttonMargin,
                              _screenHeight - _buttonHeight - _buttonMargin,
                              _buttonWidth,
                              _buttonHeight),
                "Back",
                () => { 
                    _generator.Back();
                    if (_isRunning){ 
                        _isRunning = false;
                        _runStopRestartButton.Label = "Run";
                    }     
                }
            );

            // TODO: Add a stop function
            Button runStopRestart = new Button(
                new Rectangle(horizontalCenterPos,
                              _screenHeight - _buttonHeight - _buttonMargin,
                              _buttonWidth,
                              _buttonHeight),
                "Run", 
                () => {
                    if (_generator.IsComplete){
                        _generator.Restart();
                        _runStopRestartButton.Label = "Run";
                        _framesCounter = 0;
                        _isRunning = false;
                        _runStopRestartButton.IsEnabled = true;
                        _stepButton.IsEnabled = true;
                    }
                    else if (_isRunning){
                        _runStopRestartButton.Label = "run";
                        _isRunning = false;
                    }
                    else {
                        _runStopRestartButton.Label = "stop";
                        _isRunning = true;
                    }
                }
            );

            Button step = new Button(
                new Rectangle(horizontalCenterPos + _buttonWidth + _buttonMargin,
                              _screenHeight - _buttonHeight - _buttonMargin,
                              _buttonWidth,
                              _buttonHeight),
                "Step",
                () => { _generator.Step(); }
            );

            return (back, runStopRestart, step);
        }


        private void DrawMaze(){
            for (int x = 0; x < _maze.Width; x++){
                for (int y = 0; y < _maze.Height; y++){
                    DrawCell(_maze.GetTile(x, y));
                }
            }
        }

        private void DrawCell(Tile tile){
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
        }




    } 
} 