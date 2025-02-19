using MazeGen.maze.tile;
using MazeGen.maze.wall;
using Raylib_cs;
using System.Numerics;
using MazeGen.ui.Components;
using MazeGen.maze;

namespace MazeGen.ui
{
    public class MazeWindow {

        public int ScreenHeight { get; private set; }
        public int ScreenWidth { get; private set; }

        private readonly int _mazeHeight;
        private readonly int _mazeWidth;

        private readonly ControlPanel _control_panel;
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

            _mazeWidth = _maze.Width * _cellSize;
            _mazeHeight = _maze.Height * _cellSize;

            
            _control_panel = new ControlPanel(generator, _mazeWidth, _mazeHeight ); 
            _control_panel.OnReset += () =>  _framesCounter = 0;
            
            ScreenWidth = _mazeWidth;
            ScreenHeight = _mazeHeight + _control_panel.ControlPanelHeight;
        }

    
        public void DrawFrame(Vector2 localMousePos) {
            if (!_generator.IsComplete && _control_panel.IsRunning() && _framesCounter >= _framesPerStep){
                _generator.Step();
                _framesCounter = 0;
            }
            _framesCounter++;

            _control_panel.Update(localMousePos);

            Raylib.ClearBackground(Color.White);
            DrawMaze();


            // draw border                
            Rectangle rect = new Rectangle(0, 0, _mazeWidth, _mazeHeight);
            Raylib.DrawRectangleLinesEx(rect, _wallThickness, Color.Black);
            
            _control_panel.Draw();


            Raylib.DrawRectangleV(localMousePos, new Vector2(50,50), Color.Red);
        }


        // TODO: Figure out if I need this
        // public void RunStandalone() {
        //     Raylib.InitWindow(ScreenWidth, ScreenHeight, "Maze Generator");
        //     Raylib.SetTargetFPS(60);

        //     while (!Raylib.WindowShouldClose()) {
        //         Raylib.BeginDrawing();
        //         DrawFrame();
        //         Raylib.EndDrawing();
        //     }
        //     Raylib.CloseWindow();
        // }


       

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