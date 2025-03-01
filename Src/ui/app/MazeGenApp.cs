using System.Diagnostics;
using System.Numerics;
using MazeGen.Algorithms;
using MazeGen.maze;
using MazeGen.maze.tile;
using MazeGen.maze.wall;
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
                DrawBackgroundMaze();

                switch (_currentScreen) {
                    case Screen.Start:
                        DrawStartScreen(mousePos);    
                        break;
                    case Screen.Instruction:
                        DrawInstructionScreen(mousePos);
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

        private void DrawInstructionScreen(Vector2 mousePos) {
            // Use 90% of screen width and height
            float instructionWidth = _windowWidth * 0.9f;
            float instructionHeight = _windowHeight * 0.9f;

            Rectangle instructionWindow = new Rectangle(
                (_windowWidth - instructionWidth) / 2,  
                (_windowHeight - instructionHeight) / 2, 
                instructionWidth,
                instructionHeight
            );

            float verticalSpacing = Math.Clamp(instructionWindow.Width * 0.02f, 5, 30); // TODO: Check clamp values
            float horizalSpacing = Math.Clamp(instructionWindow.Height * 0.02f, 5, 30); // TODO: Check clamp values

            // Transparrent color for the window 
            Color transColor = new Color(200, 200, 200, 240);
            Raylib.DrawRectangleRec(instructionWindow, transColor);
            Raylib.DrawRectangleLinesEx(instructionWindow, 2, Color.Black);
            
            string title = "Instructions";
            int titleFontSize = Math.Clamp((int)(instructionWindow.Width * 0.05f), 20, 70);
            float textSpacing = 2;
            Vector2 titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, titleFontSize, textSpacing);

            float titleX = instructionWindow.X + (instructionWindow.Width - titleSize.X) / 2;
            float titleY = instructionWindow.Y + horizalSpacing;
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                title,
                new Vector2(titleX, titleY),
                titleFontSize,
                textSpacing,
                Color.White
            );

            // underline title 
            Vector2 startPos = new Vector2(titleX, titleY + titleSize.Y);
            Vector2 endPos = new Vector2(titleX + titleSize.X, titleY + titleSize.Y);

            // Instruction buttons
            (string label, string desc)[] buttonDesc = new(string label, string desc)[] {
                ("Step", "Performs one step of the maze generation algorithm"),
                ("Run", "Continuously runs the algorithm until completion or stop"),
                ("Stop", "Stops the algorithm if it is currently running"),
                ("Restart", "Restarts the algorithm with a new seed"),
                ("Back", "Undoes the last step of the algorithm"),
            };


            // Calculate the width of the buttons based on the longest text and the window width
            float fontSize = Math.Clamp(instructionWindow.Width * 0.03f, 12, 30);
            float maxTextWidth = buttonDesc.Max(b => Raylib.MeasureTextEx(Raylib.GetFontDefault(), b.label, fontSize, textSpacing).X);
            float ButtonWidth = maxTextWidth + verticalSpacing;

            float minButtonWidth = instructionWindow.Width * 0.08f;
            float maxButtonWidth = instructionWindow.Width * 0.30f;
            ButtonWidth = Math.Min(Math.Max(ButtonWidth, minButtonWidth), maxButtonWidth);
            
                
            
            float currentY = titleY + titleSize.Y;
            string buttonSectionTitle = "Buttons:";
            int sectionFontSize = Math.Clamp((int)(titleFontSize * 0.70f), 12, 50);
            Vector2 buttonSectionSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), buttonSectionTitle, sectionFontSize, textSpacing); 
            
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                buttonSectionTitle,
                new Vector2(instructionWindow.X + verticalSpacing, currentY),
                sectionFontSize,
                textSpacing,
                Color.Black

            );
            currentY += buttonSectionSize.Y + horizalSpacing;
            Raylib.DrawLineEx(new Vector2(instructionWindow.X, currentY), new Vector2(instructionWindow.X + instructionWindow.Width, currentY), 3, Color.Red);

            foreach (var (label, desc) in buttonDesc) {
                currentY = DrawButtonWithDesc(label, desc, currentY, instructionWindow, verticalSpacing, horizalSpacing, textSpacing, fontSize, ButtonWidth);
            }

            string tileSectionTitle = "Tiles:";
            Vector2 tileSectionSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), tileSectionTitle, sectionFontSize, textSpacing); 
            
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                tileSectionTitle,
                new Vector2(instructionWindow.X + verticalSpacing, currentY),
                sectionFontSize,
                textSpacing,
                Color.Black
            );
            currentY += tileSectionSize.Y + horizalSpacing;
            // instruction tiles
            (Color color, String desc) [] tileDesc = new(Color color, String desc)[] {
                (Color.Gray, "The algorithm has not visited this tile"),
                (Color.LightGray, "The algorithm has visited this tile"),
                (Color.White, "The algorithm has selected this tile for the final maze"),
                (Color.Red, "The current tile the algorithm is working on")
            };

            foreach (var (color, desc) in tileDesc) {
                currentY = DrawTileWithDesc(color, desc, currentY, instructionWindow, verticalSpacing, horizalSpacing, textSpacing, fontSize, ButtonWidth);
            }

            // Draw close button
            float exitButtonScaleFactor = 0.6f;
            Vector2 buttonTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), "Exit", fontSize * exitButtonScaleFactor, textSpacing);
            float exitButtonHeight = (buttonTextSize.Y + verticalSpacing) * exitButtonScaleFactor;
            float exitButtonWidth = ButtonWidth * exitButtonScaleFactor; 
            Button exitButton = new Button(
                instructionWindow.X + instructionWindow.Width - verticalSpacing - exitButtonWidth,
                instructionWindow.Y + horizalSpacing,
                exitButtonWidth,
                exitButtonHeight,
                "Exit",
                fontSize * exitButtonScaleFactor,
                () => _currentScreen = Screen.Start
            );
            exitButton.Update(mousePos);
            exitButton.Draw();

        }

        private float DrawTileWithDesc(Color color, String desc, float currentY, Rectangle window, float vSpace, float hSpace, float textSpacing, float fontSize, float buttonWidth) { 
            Vector2 descSize= Raylib.MeasureTextEx(Raylib.GetFontDefault(), desc, fontSize, textSpacing);
            float tileX =  window.X + vSpace;
            float tileSize = descSize.Y + vSpace;

            float textStartX = tileX + buttonWidth + hSpace;
            float textStartY = currentY + (tileSize - fontSize) / 2;
            Rectangle rect = new Rectangle(
                    tileX,
                    currentY,
                    tileSize,
                    tileSize
            );
            if (color.Equals(Color.Red)) {
                Raylib.DrawRectangleRec(rect, Color.Blank);
                float centerX = tileX + tileSize / 2;
                float centerY = currentY + tileSize / 2;
                float radius = tileSize / 4;
                Raylib.DrawCircle((int)centerX, (int)centerY, radius, Color.Red);
            } else {
                Raylib.DrawRectangleRec(rect, color);
            }


            Raylib.DrawRectangleLinesEx(rect, BACKGROUND_WALL_THICKNESS, Color.Black);

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                desc,
                new Vector2(textStartX, textStartY),
                fontSize,
                textSpacing,
                Color.Black
            );

            return currentY + tileSize + hSpace;
        }

        private float DrawButtonWithDesc(string buttonText, string description, float currentY, Rectangle window, float vSpace, float hSpace, float textSpacing, float fontSize, float buttonWidth) {  
            Vector2 buttonTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), buttonText, fontSize, textSpacing);
            
            float buttonHeight = buttonTextSize.Y + vSpace;  // half vspace padding on each side
            float buttonX = window.X + vSpace;
            float buttonY = currentY;

            float descStartX = buttonX + buttonWidth + hSpace; 
            float descStartY = buttonY + (buttonHeight - fontSize) / 2; 

            Button button = new Button(
                buttonX, buttonY, buttonWidth, buttonHeight,
                buttonText,
                fontSize,
                () => { }
            );
            button.Draw();

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                description,
                new Vector2(descStartX, descStartY),
                fontSize,
                textSpacing,
                Color.Black
            );

            return currentY + buttonHeight + hSpace;
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