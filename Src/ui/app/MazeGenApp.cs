using System.Diagnostics;
using System.Numerics;
using MazeGen.Algorithms;
using MazeGen.maze;
using MazeGen.maze.tile;
using MazeGen.maze.wall;
using MazeGen.ui.components;
using Raylib_cs;

namespace MazeGen.ui.app {

    public enum Screen {
        Start,
        Instruction, 
        Maze
    }

    public class MazeGenApp {
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly MazeWindow[] _mazeWindows;
        private RenderTexture2D[] _renderTextures;
        private Screen _currentScreen = Screen.Start;
        

        private const int BUTTON_WIDTH = 200;

        private const int BUTTON_HEIGHT = 50;
        private const int MAZE_PADDING = 10;

        private ScreenInstruction _instructionScreen;
        private ScreenStart _startScreen;

        public MazeGenApp(MazeWindow[] mazeDraws) {
            _mazeWindows = mazeDraws;
            _renderTextures = new RenderTexture2D[_mazeWindows.Length];

            _windowWidth = (_mazeWindows[0].ScreenWidth * mazeDraws.Length) + (MAZE_PADDING * (mazeDraws.Length + 1)); // +1 for the outer edges
            _windowHeight = _mazeWindows[0].ScreenHeight + (2 * MAZE_PADDING);

            _instructionScreen = new ScreenInstruction(_windowWidth, _windowHeight, () => _currentScreen = Screen.Start);
            _startScreen = new ScreenStart(_windowWidth, _windowHeight, () => _currentScreen = Screen.Maze, () => _currentScreen = Screen.Instruction, () => Debug.WriteLine("Settings clicked"));
        }

        private void InitializeRenderTextures() {
            for (int i = 0; i < _mazeWindows.Length; i++) {
                _renderTextures[i] = Raylib.LoadRenderTexture(
                    _mazeWindows[i].ScreenWidth, 
                    _mazeWindows[i].ScreenHeight
                );
            }
        }


        public void Run() {
            Raylib.InitWindow(_windowWidth, _windowHeight, "Maze Generator");
            Raylib.SetTargetFPS(60);
            InitializeRenderTextures();

            _instructionScreen.Initialize();
            _startScreen.Initialize();

            while (!Raylib.WindowShouldClose()) {
                Vector2 mousePos = Raylib.GetMousePosition();

                Raylib.BeginDrawing();

                if (_currentScreen != Screen.Maze) {
                    _startScreen.DrawBackgroundMaze();
                }

                switch (_currentScreen) {
                    case Screen.Start:
                        _startScreen.Draw(mousePos);
                        break;
                    case Screen.Instruction:
                        _instructionScreen.Draw(mousePos);
                        break;
                    case Screen.Maze:
                        Raylib.ClearBackground(Color.White);
                        DrawMazeScreen(mousePos);
                        break;
                }

                Raylib.EndDrawing();

            }

            Cleanup();
        }


        private void DrawMazeScreen(Vector2 mousePos) {
            int totalWidth = (_mazeWindows[0].ScreenWidth * _mazeWindows.Length) +
                            (MAZE_PADDING * (_mazeWindows.Length - 1));
            int startX = (_windowWidth - totalWidth) / 2;
            

            for (int i = 0; i < _mazeWindows.Length; i++) {
                 // Calculate the offset for the current MazeDraw's render texture
                Vector2 offset = new Vector2(
                    startX + (i * (_mazeWindows[i].ScreenWidth + MAZE_PADDING)),
                    MAZE_PADDING
                );
                 // Convert the global mouse position to local coordinates for this MazeDraw
                Vector2 localMousePos = mousePos - offset;

                Raylib.BeginTextureMode(_renderTextures[i]);
                    _mazeWindows[i].DrawFrame();
                Raylib.EndTextureMode();
            
            }


            for (int i = 0; i < _mazeWindows.Length; i++) {

                Rectangle sourceRect = new Rectangle(
                    0,
                    0,
                    _renderTextures[i].Texture.Width,
                    -_renderTextures[i].Texture.Height
                ); 
                
                Rectangle destRect = new Rectangle(
                    startX + (i * (_mazeWindows[i].ScreenWidth + MAZE_PADDING)),
                    MAZE_PADDING, 
                    _mazeWindows[i].ScreenWidth,
                    _mazeWindows[i].ScreenHeight
                );

                Raylib.DrawTexturePro(
                   _renderTextures[i].Texture, 
                   sourceRect,  
                   destRect,
                   Vector2.Zero, 
                   0f,
                   Color.White
                );

            //     _exitButton.Rect = new Rectangle(
            //         destRect.X + destRect.Width - _exitButton.Rect.Width - MAZE_PADDING,
            //         destRect.Y + MAZE_PADDING, 
            //         _exitButton.Rect.Width,
            //         _exitButton.Rect.Height
            //     );
            //     _exitButton.Draw();
            //     _exitButton.Update(mousePos);
            }
            
        }

        private void Cleanup() {
            for (int i = 0; i < _renderTextures.Length; i++){
                Raylib.UnloadRenderTexture(_renderTextures[i]);
            }
            Raylib.CloseWindow();
        }

       
    }
}