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

        // Background maze 
        private Maze _backgroundMaze; 
        private IGenerator _backgroundGenerator;
        private const int BACKGROUND_CELL_SIZE = 30;
        private const int BACKGROUND_WALL_THICKNESS = 3;
        private const int FRAMES_PER_STEP = 4;
        private int _frameCounter = 0;

        private const int BUTTON_WIDTH = 200;

        private const int BUTTON_HEIGHT = 50;
        private const int MAZE_PADDING = 10;

        // private float _instructionScrollY = 0;
        // private float _totalInstructionContentHeight = 0;
        private ScreenInstruction _instructionScreen;

        public MazeGenApp(MazeWindow[] mazeDraws) {
            _mazeWindows = mazeDraws;
            _renderTextures = new RenderTexture2D[_mazeWindows.Length];

            _windowWidth = (_mazeWindows[0].ScreenWidth * mazeDraws.Length) + (MAZE_PADDING * (mazeDraws.Length + 1)); // +1 for the outer edges
            _windowHeight = _mazeWindows[0].ScreenHeight + (2 * MAZE_PADDING);


            // init background maze
            int dimx = (int)Math.Ceiling((float)_windowWidth / BACKGROUND_CELL_SIZE);
            int dimy = (int)Math.Ceiling((float)_windowHeight / BACKGROUND_CELL_SIZE);
            _backgroundMaze = new Maze(dimx, dimy);
            _backgroundGenerator = new Backtracking(_backgroundMaze);


            _instructionScreen = new ScreenInstruction(_windowWidth, _windowHeight, () => _currentScreen = Screen.Start);
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

            while (!Raylib.WindowShouldClose()) {
                Vector2 mousePos = Raylib.GetMousePosition();

                Raylib.BeginDrawing();
                DrawBackgroundMaze();

                switch (_currentScreen) {
                    case Screen.Start:
                        DrawStartScreen(mousePos);    
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


        private void DrawStartScreen(Vector2 mousePos){
            // Title 
            string title = "Maze Generator";
            int titleFontsize = (int)(_windowWidth * 0.1f); 
            titleFontsize = Math.Clamp(titleFontsize, 15, 140);
            int padding = 10;
            int textWidth = Raylib.MeasureText(title, titleFontsize);
            
            float rectWidth = textWidth + (2 * padding);
            float rectHeight = titleFontsize + (2 * padding);
            Rectangle titleRect = new Rectangle(
                (_windowWidth / 2) - (rectWidth / 2) ,
                (_windowHeight / 4) - (rectHeight / 2),
                rectWidth,
                rectHeight
            );
            Raylib.DrawRectangleRec(titleRect, Color.SkyBlue);
            Raylib.DrawRectangleLinesEx(titleRect, 2, Color.Black);
            
            Raylib.DrawText(
                title,
                (int)titleRect.X + padding,
                (int)titleRect.Y + padding,
                titleFontsize,
                Color.White
            );


            // Menu buttons
            int verticalSpacing = (int)(_windowHeight * 0.05f);
            verticalSpacing = Math.Clamp(verticalSpacing, 20, 100);
            float baseY = titleRect.Y + rectHeight + verticalSpacing;

            (string Label, Action onClick)[] buttonDefinitions = new(string, Action)[] {
                ("Start", () => _currentScreen = Screen.Maze),
                ("Settings", () => { /* TODO: Implement settings */ }),
                ("Instructions", () => _currentScreen = Screen.Instruction)
            };

            List<Button> buttons = new();
            for (int i = 0; i < buttonDefinitions.Length; i++) {
                var (text, action) = buttonDefinitions[i];
                buttons.Add(CreateMenuButton(text, titleFontsize, padding, baseY, i, verticalSpacing, action));
            }

            foreach (Button button in buttons) {
                button.Update(mousePos);
                button.Draw();
            }
        }


        private Button CreateMenuButton( string buttonText, int titleFontsize, int padding, float baseY, int buttonIndex, int verticalSpacing, Action onClick) {
            int menuButtonsFont = (int)(titleFontsize * 0.3f);
            menuButtonsFont = Math.Clamp(menuButtonsFont, 12, 40);

            int buttonTextWidth = Raylib.MeasureText(buttonText, menuButtonsFont);
            int buttonWidth = buttonTextWidth + (2 * padding);
            int buttonHeight = menuButtonsFont + (2 * padding);

            int buttonY = (int)(baseY + (buttonHeight + verticalSpacing) * buttonIndex);

            return new Button(
                (_windowWidth / 2) - (buttonWidth / 2),
                buttonY,
                buttonWidth,
                buttonHeight,
                buttonText,
                menuButtonsFont,
                onClick
            );
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

        private void DrawBackgroundMaze() {
            if (!_backgroundGenerator.IsComplete && _frameCounter >= FRAMES_PER_STEP) {
                _backgroundGenerator.Step();
                _frameCounter = 0;
            }
            _frameCounter++;

        
            for (int x = 0; x < _backgroundMaze.Width; x++) {
                for (int y = 0; y < _backgroundMaze.Height; y++) {
                    Tile tile = _backgroundMaze.GetTile(x, y);
                    int posX = x * BACKGROUND_CELL_SIZE;
                    int posY = y * BACKGROUND_CELL_SIZE;

                    
                    // Draw cell with very light gray for visited cells
                    // Color cellColor = tile.State == TileState.Visited ? new Color(245, 245, 245, 255) : Color.White;
                    Raylib.DrawRectangle(posX, posY, BACKGROUND_CELL_SIZE, BACKGROUND_CELL_SIZE, tile.Color);

                    // Draw walls
                    if (_backgroundMaze.HasWall(tile, Wall.North)) {
                        Vector2 v1 = new Vector2(posX, posY);
                        Vector2 v2 = new Vector2(posX + BACKGROUND_CELL_SIZE, posY);
                        Raylib.DrawLineEx(v1, v2, BACKGROUND_WALL_THICKNESS, Color.Black);
                    }
                    
                    if (_backgroundMaze.HasWall(tile, Wall.East)) {
                        Vector2 v1 = new Vector2(posX + BACKGROUND_CELL_SIZE, posY);
                        Vector2 v2 = new Vector2(posX + BACKGROUND_CELL_SIZE, posY + BACKGROUND_CELL_SIZE);
                        Raylib.DrawLineEx(v1, v2, BACKGROUND_WALL_THICKNESS, Color.Black);
                    }

                    if (_backgroundMaze.HasWall(tile, Wall.South)) {
                        Vector2 v1 = new Vector2(posX, posY + BACKGROUND_CELL_SIZE);
                        Vector2 v2 = new Vector2(posX + BACKGROUND_CELL_SIZE, posY + BACKGROUND_CELL_SIZE);
                        Raylib.DrawLineEx(v1, v2, BACKGROUND_WALL_THICKNESS, Color.Black);
                    }

                    if (_backgroundMaze.HasWall(tile, Wall.West)) {
                        Vector2 v1 = new Vector2(posX, posY);
                        Vector2 v2 = new Vector2(posX, posY + BACKGROUND_CELL_SIZE);
                        Raylib.DrawLineEx(v1, v2, BACKGROUND_WALL_THICKNESS, Color.Black);
                    }

                    if (_backgroundGenerator.currentTile == tile) {
                        int centerX = posX + BACKGROUND_CELL_SIZE / 2;
                        int centerY = posY + BACKGROUND_CELL_SIZE / 2;
                        int radius = BACKGROUND_CELL_SIZE / 4;
                        Raylib.DrawCircle(centerX, centerY, radius, Color.Red); 
                    }
                }
            } 
        }
    }
}