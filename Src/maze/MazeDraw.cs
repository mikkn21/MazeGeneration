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
        private readonly int _cellSize; // Size of each cell in pixels
        private readonly int _wallThickness; // Thickness of the walls in pixels


        // Button properties
        private const int _buttonWidth = 150;
        private const int _buttonHeight = 40;
        private const int _buttonMargin = 20;
        private readonly Color _buttonColor = Color.SkyBlue;
        private readonly Color _textColor = Color.White;
        private readonly string _buttonText = "Step";

        public MazeDraw(Maze maze, int cellSize, int wallThickness = 3, int framesPerStep = 1){
            _maze = maze;
            _cellSize = cellSize;
            _wallThickness = wallThickness;
            _framesPerStep = framesPerStep;
        }

    
        public void Draw(IGenerator generator) {
            int screenWidth = _maze.Width * _cellSize;
            int screenHeight = _maze.Height * _cellSize + _buttonHeight + _buttonMargin;
            Raylib.InitWindow(screenWidth, screenHeight, "Maze Generator");
            Raylib.SetTargetFPS(60);

            int framesCounter = 0;          

            // Main rendering loop
            while (!Raylib.WindowShouldClose()){
                Vector2 mousePos = Raylib.GetMousePosition();


                if (!generator.IsComplete && framesCounter >= _framesPerStep){
                    generator.Step();
                    framesCounter = 0;
                }

                framesCounter++;

                Rectangle buttonRect = new Rectangle(
                        (screenWidth - _buttonWidth) / 2, // Center the button horizontally
                        screenHeight - _buttonHeight - _buttonMargin, // Position at the bottom
                        _buttonWidth,
                        _buttonHeight
                    );

                Raylib.DrawRectangleRec(buttonRect, _buttonColor);
                Raylib.DrawText(
                    _buttonText,
                    (int)buttonRect.X + 20, // Adjust text position
                    (int)buttonRect.Y + 10,  // Adjust text position
                    20, // Font size
                    _textColor
                );

                // Check if the button was clicked
                if (Raylib.CheckCollisionPointRec(mousePos, buttonRect) && Raylib.IsMouseButtonPressed(MouseButton.Left)){
                    Console.WriteLine("Button clicked");
                }



                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);

                DrawMaze();


                // draw border                
                Rectangle rect = new Rectangle(0, 0, screenWidth, screenHeight);
                Raylib.DrawRectangleLinesEx(rect, _wallThickness, Color.Black);


                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
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