using MazeGen.maze.wall;
using Raylib_cs;
using System.Numerics;

namespace MazeGen.maze.draw
{
    public class MazeDraw {
        private Maze _maze;

        // 60 -> 1 step per second
        // 30 -> 1 step per 2 seconds
        // 10 -> 1 step per 6 seconds
        private int _framesPerStep; 
        private readonly int _cellSize; // Size of each cell in pixels
        private readonly int _wallThickness; // Thickness of the walls in pixels
        


        public MazeDraw(Maze maze, int cellSize, int wallThickness = 2, int framesPerStep = 10){
            _maze = maze;
            _cellSize = cellSize;
            _wallThickness = wallThickness;
            _framesPerStep = framesPerStep;
        }

    
        public void Draw(IGenerator generator) {
            int screenWidth = _maze.Width * _cellSize;
            int screenHeight = _maze.Height * _cellSize;
            Raylib.InitWindow(screenWidth, screenHeight, "Maze Generator");
            Raylib.SetTargetFPS(60);

            int framesCounter = 0;          

            // Main rendering loop
            while (!Raylib.WindowShouldClose()){

                if (!generator.IsComplete && framesCounter >= _framesPerStep){
                    generator.Step();
                    framesCounter = 0;
                }

                framesCounter++;


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
                    DrawCell(x, y);
                }
            }
        }

        private void DrawCell(int x, int y){
            int posX = x * _cellSize;
            int posY = y * _cellSize;

            // Draw walls
            if (_maze.HasWall(x, y, Wall.North)){
                Vector2 v1 = new Vector2(posX, posY);
                Vector2 v2 = new Vector2(posX + _cellSize, posY);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }
            if (_maze.HasWall(x, y, Wall.East)){
                Vector2 v1 = new Vector2(posX + _cellSize, posY);
                Vector2 v2 = new Vector2(posX + _cellSize, posY + _cellSize);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }
            if (_maze.HasWall(x, y, Wall.South)){
                Vector2 v1 = new Vector2(posX, posY + _cellSize);
                Vector2 v2 = new Vector2(posX + _cellSize, posY + _cellSize);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }
            if (_maze.HasWall(x, y, Wall.West)){
                Vector2 v1 = new Vector2(posX, posY);
                Vector2 v2 = new Vector2(posX, posY + _cellSize);
                Raylib.DrawLineEx(v1, v2, _wallThickness, Color.Black);
            }
        }




    } // class MazeDraw
} // namespace MazeGen.maze.draw