using System.Numerics;
using MazeGen.ui.Components;
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

        // Start screen elements 
        private Button _startButton;
        private Button _instructionButton;
        private Button _settingsButton;
        private const int BUTTON_WIDTH = 200;

        private const int BUTTON_HEIGHT = 50;
        private const int MAZE_PADDING = 10;


        public MazeGenApp(MazeWindow[] mazeDraws) {
            _mazeWindows = mazeDraws;
            _renderTextures = new RenderTexture2D[_mazeWindows.Length];

            _windowWidth = (_mazeWindows[0].ScreenWidth * mazeDraws.Length) + (MAZE_PADDING * (mazeDraws.Length + 1)); // +1 for the outer edges
            _windowHeight = _mazeWindows[0].ScreenHeight + (2 * MAZE_PADDING);

            (_startButton, _instructionButton ,_settingsButton) = InitializeUI();
        }

        private (Button start, Button instruction, Button setting) InitializeUI() {
            int centerX = _windowWidth / 2 - BUTTON_WIDTH / 2;
            int centerY = _windowHeight / 2 - BUTTON_HEIGHT / 2;

            Button start = new Button(
                new Rectangle (centerX, centerY, BUTTON_WIDTH, BUTTON_HEIGHT),
                "Start",
                () => _currentScreen = Screen.Maze 
            );

            Button instructions = new Button( 
                new Rectangle (centerX, centerY + (BUTTON_HEIGHT + 20), BUTTON_WIDTH, BUTTON_HEIGHT),
                "Instructions",
                () => { /* TODO: Implment instructions */ }
            );

            Button settings = new Button(
                new Rectangle(centerX, centerY + 2 * (BUTTON_HEIGHT + 20), BUTTON_WIDTH, BUTTON_HEIGHT),
                "Settings",
                () => { /* TODO: Implement settings */ }
            );

            return (start, instructions, settings);
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

            while (!Raylib.WindowShouldClose()) {
                
                Vector2 mousePos = Raylib.GetMousePosition();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);

                switch (_currentScreen) {
                    case Screen.Start:
                        DrawStartScreen(mousePos);    
                        break;
                    case Screen.Maze:
                        DrawMazeScreen(mousePos);
                        break;
                }

                Raylib.EndDrawing();

            }

            Cleanup();
        }


        private void DrawStartScreen(Vector2 mousePos){
            _startButton.Update(mousePos);
            _instructionButton.Update(mousePos);
            _settingsButton.Update(mousePos);

            string title = "Maze Generator";
            int fontsize = 40;
            Vector2 titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, fontsize, 2);
            Raylib.DrawText(
                title,
                _windowWidth / 2 - (int)titleSize.X / 2,
                _windowHeight / 4,
                fontsize,
                Color.Black
            );


            _startButton.Draw();   
            _instructionButton.Draw();
            _settingsButton.Draw();


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
                    _mazeWindows[i].DrawFrame(localMousePos);
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