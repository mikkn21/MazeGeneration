using System.Numerics;
using MazeGen.Algorithms;
using MazeGen.maze;
using Raylib_cs;

namespace MazeGen.ui.components {

    public enum MazeLayout {
        OneMaze,
        TwoMazes,
        ThreeMazes,
        FourMazes
    }


    public class ScreenMaze {

        private MazeWindow[] _mazeWindows;
        private ControlPanel[] _controlPanels;
        private RenderTexture2D[] _renderTextures;
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private int _mazeCount;
        private const int MAZE_PADDING = 10;

        const float CONTROL_PANE_HEIGHT_SCALE = 0.10f; 
       
        private int _controlPanelHeight;

        public MazeLayout CurrentLayout { get; set; }

        private int _mazeWidth;
        private int _mazeHeight;

        public ScreenMaze(int windowWidth, int windowHeight, MazeLayout layout, int mazeWidth = 10, int mazeHeight = 10) {
            _mazeWindows = Array.Empty<MazeWindow>();
            _renderTextures = Array.Empty<RenderTexture2D>();
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;   
            CurrentLayout = layout;

            _mazeWidth = mazeWidth;
            _mazeHeight = mazeHeight;
            _mazeCount = CurrentLayout switch {
                MazeLayout.OneMaze => 1,
                MazeLayout.TwoMazes => 2,
                MazeLayout.ThreeMazes => 3,
                MazeLayout.FourMazes => 4,
                _ => 2
            };

            _mazeWindows = new MazeWindow[_mazeCount];
            _controlPanels = new ControlPanel[_mazeCount];
            _renderTextures = new RenderTexture2D[_mazeCount]; 
        
        }

        public void Initialize() {
            _controlPanelHeight = CalculateControlPanelHeight();
            int cellSize = CalculateCellSize(_mazeWidth, _mazeHeight);
            
            int wallThickness = Math.Max(1, cellSize / 10); // Proportional to cell size


            for (int i = 0; i < _mazeCount; i++) {
                Maze maze = new Maze (_mazeWidth, _mazeHeight); 

                IGenerator generator = new Backtracking(maze);
                 
                _mazeWindows[i] = new MazeWindow( 
                    maze,
                    cellSize, 
                    generator,
                    wallThickness, 
                    5 // TODO: The frames per step should be an argument to the constructor 
                );

                _controlPanels[i] = new ControlPanel(_mazeWindows[i], _mazeWindows[i].Width, _controlPanelHeight);

                _renderTextures[i] = Raylib.LoadRenderTexture(
                    _mazeWindows[i].Width,
                    _mazeWindows[i].Height + _controlPanels[i].Height
                );
            }
        }

        private int CalculateCellSize(int mazeWidth, int mazeHeight) {
            int availableWidth, availableHeight;

            switch (CurrentLayout) {
                case MazeLayout.OneMaze: 
                    availableWidth = _windowWidth - (2 * MAZE_PADDING);
                    availableHeight = _windowHeight - _controlPanelHeight - (2 * MAZE_PADDING);
                    break;

                case MazeLayout.TwoMazes:
                    availableWidth = (_windowWidth / 2) - (3 * MAZE_PADDING);
                    availableHeight = _windowHeight - _controlPanelHeight - (2 * MAZE_PADDING);
                    break;
                
                case MazeLayout.ThreeMazes:
                case MazeLayout.FourMazes: 
                    availableWidth = (_windowWidth  / 2) - (3 * MAZE_PADDING);
                    availableHeight = (_windowHeight/2) - _controlPanelHeight - (2 * MAZE_PADDING);
                    break;
                
                default: 
                    availableWidth = _windowWidth / 2;
                    availableHeight = (_windowHeight - _controlPanelHeight) /2;
                    break;
            }

            int cellWidthSize = availableWidth / mazeWidth;
            int cellHeightSize = availableHeight / mazeHeight;

            return Math.Min(cellWidthSize, cellHeightSize);
        }

        private int CalculateControlPanelHeight() {
            int availableHeight;

            switch (CurrentLayout)
            {
                case MazeLayout.OneMaze:
                case MazeLayout.TwoMazes:
                    // For 1-row layouts, use percentage of window height
                    availableHeight = _windowHeight;
                    break;

                case MazeLayout.ThreeMazes:
                case MazeLayout.FourMazes:
                    // For 2x2 layouts, use percentage of half window height
                    availableHeight = _windowHeight / 2;
                    break;

                default:
                    availableHeight = _windowHeight;
                    break;
            }
            
            int panelHeight = (int)(availableHeight * CONTROL_PANE_HEIGHT_SCALE);
            // Consider if this is necessary
            // panelHeight = Math.Max(30, Math.Min(60, panelHeight));
            

            return panelHeight;
        }


        public void Draw(Vector2 mousePos) {
            Rectangle[] destRects = CalculateDestRects();

            for (int i = 0; i < _mazeWindows.Length; i++) {
                Vector2 offset = new Vector2(destRects[i].X, destRects[i].Y);
                Vector2 localMousePos = mousePos - offset;

                Raylib.BeginTextureMode(_renderTextures[i]);
                _mazeWindows[i].DrawFrame();
                Raylib.EndTextureMode();

            }
            

            // draw textures
            for (int i = 0; i < _mazeWindows.Length; i++) {
                Rectangle sourceRect = new Rectangle(
                    0,
                    0,
                    _renderTextures[i].Texture.Width,
                    -_renderTextures[i].Texture.Height
                );


                Raylib.DrawTexturePro(
                   _renderTextures[i].Texture,
                   sourceRect,
                   destRects[i],
                   Vector2.Zero,
                   0f,
                   Color.White
                );
            }

            // TODO: Find out if can be drawn in one of the other loops.
            for (int i = 0; i < _controlPanels.Length; i++) {
                _controlPanels[i].Position = new Vector2 ( 
                    destRects[i].X,
                    destRects[i].Y + _mazeWindows[i].Height + 5 // 5 is padding between maze and control panel
                );
                _controlPanels[i].Update(mousePos);
                _controlPanels[i].Draw(); 
            }

            //     _exitButton.Rect = new Rectangle(
            //         destRect.X + destRect.Width - _exitButton.Rect.Width - MAZE_PADDING,
            //         destRect.Y + MAZE_PADDING, 
            //         _exitButton.Rect.Width,
            //         _exitButton.Rect.Height
            //     );
            //     _exitButton.Draw();
            //     _exitButton.Update(mousePos);


        }

        private Rectangle[] CalculateDestRects() {
            Rectangle[] destRects = new Rectangle[_mazeWindows.Length];
            int mazeWidth = _mazeWindows[0].Width;
            int mazeHeight = _mazeWindows[0].Height;
            int controlHeight = _controlPanels[0].Height;

            int totalHeightPerMaze = mazeHeight + controlHeight;

            if (CurrentLayout == MazeLayout.FourMazes || CurrentLayout == MazeLayout.ThreeMazes) {
                // For a 2x2 grid layout:
                int gridWidth = (mazeWidth * 2) + (3 * MAZE_PADDING); 
                int controlPanelPadding = 5;
                int gridHeight = (totalHeightPerMaze * 2) + controlPanelPadding + (3 * MAZE_PADDING); // 2 rows, 1 gap
                int startX = (_windowWidth - gridWidth) / 2;
                int startY = (_windowHeight - gridHeight) / 2;
                
                

                for (int i = 0; i < _mazeCount; i++) {
                    int row = i / 2;
                    int col = i % 2;
                    destRects[i] = new Rectangle(
                        startX + MAZE_PADDING + (col * (mazeWidth + MAZE_PADDING)), 
                        startY + MAZE_PADDING + (row * (totalHeightPerMaze + MAZE_PADDING)), 
                        mazeWidth,
                        totalHeightPerMaze
                    );
                }
            }
            else {
                // For a single row layout (OneMaze or TwoMazes):
                int paddingSpaces = _mazeWindows.Length + 1;
    
                int gridWidth = (mazeWidth * _mazeWindows.Length) + (paddingSpaces * MAZE_PADDING); 
                int gridHeight = totalHeightPerMaze + (2  * MAZE_PADDING); 
                int startX = (_windowWidth - gridWidth) / 2;
                int startY = (_windowHeight - gridHeight) / 2;


                for (int i = 0; i < _mazeWindows.Length; i++) {
                    destRects[i] = new Rectangle(
                        startX + MAZE_PADDING + (i * (mazeWidth + MAZE_PADDING)),
                        startY + MAZE_PADDING,
                        mazeWidth,
                        totalHeightPerMaze
                    );
                }
            }

            return destRects;
        }


        public void Cleanup() {
            for (int i = 0; i < _renderTextures.Length; i++) {
                Raylib.UnloadRenderTexture(_renderTextures[i]);
            }
        }



    }
}