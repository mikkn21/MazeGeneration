using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using MazeGen.ui.components;
using MazeGen.ui.components.screens;
using Raylib_cs;

namespace MazeGen.ui.app {

    public enum Screen {
        Start,
        Settings,
        Instruction, 
        Maze
    }

    public class MazeGenApp : IDisposable {
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly MazeWindow[] _mazeWindows;
        private RenderTexture2D[] _renderTextures;
        private Screen _currentScreen = Screen.Start;
        

        private const int BUTTON_WIDTH = 200;

        private const int BUTTON_HEIGHT = 50;
        private const int MAZE_PADDING = 10;

        private IScreen _instructionScreen;
        private IScreen _settingsScreen;
        private IScreen _startScreen;

        private ScreenMaze _mazeScreen;

        private bool _disposed = false;
        private MazeSettingsModel _settingsManager;

        public MazeGenApp(MazeWindow[] mazeDraws) {
            _mazeWindows = mazeDraws;
            _renderTextures = new RenderTexture2D[_mazeWindows.Length];

            _windowWidth = (_mazeWindows[0].Width * mazeDraws.Length) + (MAZE_PADDING * (mazeDraws.Length + 1)); // +1 for the outer edges
            _windowHeight = _mazeWindows[0].Height + (2 * MAZE_PADDING);

            MazeSettings defaultSettings = new MazeSettings();
            _settingsManager = new MazeSettingsModel(defaultSettings);

            _settingsManager.SettingsChanged += OnSettingsChanged;

            _instructionScreen = new ScreenInstruction(_windowWidth, _windowHeight, () => _currentScreen = Screen.Start);
            _mazeScreen = new ScreenMaze(_windowWidth, _windowHeight, () => _currentScreen = Screen.Start, _settingsManager);
            _settingsScreen = new ScreenSettings(_windowWidth, _windowHeight, () => _currentScreen = Screen.Start , _settingsManager);

            _startScreen = new ScreenStart(_windowWidth, _windowHeight, () => _currentScreen = Screen.Maze, () => _currentScreen = Screen.Instruction, () => _currentScreen = Screen.Settings);
            
        }

        private void InitializeRenderTextures() {
            for (int i = 0; i < _mazeWindows.Length; i++) {
                _renderTextures[i] = Raylib.LoadRenderTexture(
                    _mazeWindows[i].Width, 
                    _mazeWindows[i].Height
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

                if (_currentScreen != Screen.Maze) {
                    _startScreen.DrawBackgroundMaze();
                }


                // TODO: REMOVE FOR DEBUG: 
                // _currentScreen = Screen.Maze;
                
                switch (_currentScreen) {
                    case Screen.Start:
                        _startScreen.Draw(mousePos);
                        break;
                    case Screen.Settings: 
                        _settingsScreen.Draw(mousePos);
                        break;
                    case Screen.Instruction:
                        _instructionScreen.Draw(mousePos);
                        break;
                    case Screen.Maze:
                        Raylib.ClearBackground(Color.White);
                        _mazeScreen.Draw(mousePos);
                        break;
                }

                Raylib.EndDrawing();

            }

            this.Dispose();
            Raylib.CloseWindow();
        }       


        private void OnSettingsChanged(object? sender, MazeSettingsChangedEventArgs e) {
            _mazeScreen.UpdateSettings(e.NewSettings);
        }


        // Unsubscribe from events and dispose of resources
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                _settingsManager.SettingsChanged -= OnSettingsChanged;

                if (_mazeScreen is IDisposable disposableMaze) {
                    disposableMaze.Dispose();
                }

                _settingsScreen.Cleanup(); 
            }

            _disposed = true;
        }
    }
}