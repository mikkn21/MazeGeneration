using System.Numerics;
using System.Runtime.InteropServices;
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
        private RenderTexture2D[] _renderTextures;
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private int _mazeCount;
        private const int MAZE_PADDING = 10;

        public MazeLayout CurrentLayout { get; set; }


        public ScreenMaze(int windowWidth, int windowHeight, MazeLayout layout) {
            _mazeWindows = Array.Empty<MazeWindow>();
            _renderTextures = Array.Empty<RenderTexture2D>();
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;   
            CurrentLayout = layout;

        }

        public void Initialize() {
            _mazeCount = CurrentLayout switch {
                MazeLayout.OneMaze => 1,
                MazeLayout.TwoMazes => 2,
                MazeLayout.ThreeMazes => 3,
                MazeLayout.FourMazes => 4,
                _ => 2
            };

            _mazeWindows = new MazeWindow[_mazeCount];
            _renderTextures = new RenderTexture2D[_mazeCount];

            int mazeWidth = 10; // TODO: The maze size should be an argument to the constructor
            int mazeHeight = 10; // TODO: The maze size should be an argument to the constructor

            int cellSize = CalculateCellSize(mazeWidth, mazeHeight);
            
            int wallThickness = Math.Max(1, cellSize / 10); // Proportional to cell size


            for (int i = 0; i < _mazeCount; i++) {
                Maze maze = new Maze (mazeWidth, mazeHeight); 

                IGenerator generator = new Backtracking(maze);
                 
                _mazeWindows[i] = new MazeWindow( 
                    maze,
                    cellSize, 
                    generator,
                    wallThickness, 
                    5 // TODO: The frames per step should be an argument to the constructor 
                );

                _renderTextures[i] = Raylib.LoadRenderTexture(
                    _mazeWindows[i].ScreenWidth,
                    _mazeWindows[i].ScreenHeight
                );
            }

        }

        private int CalculateCellSize(int mazeWidth, int mazeHeight) {
            int availableWidth, availableHeight;

            switch (CurrentLayout) {
                case MazeLayout.OneMaze: 
                    availableWidth = _windowWidth - (2 * MAZE_PADDING);
                    availableHeight = _windowHeight - (2 * MAZE_PADDING);
                    break;

                case MazeLayout.TwoMazes:
                    availableWidth = (_windowWidth - (3 * MAZE_PADDING)) / 2;
                    availableHeight = _windowHeight - (2 * MAZE_PADDING);
                    break;
                
                case MazeLayout.ThreeMazes:
                case MazeLayout.FourMazes: 
                    availableWidth = (_windowWidth - (4 * MAZE_PADDING)) / 3;
                    availableHeight = (_windowHeight - (3 * MAZE_PADDING)) / 2;
                    break;
                
                default: 
                    availableWidth = _windowWidth / 2;
                    availableHeight = _windowHeight /2;
                    break;
            }

            int cellWidthSize = availableWidth / mazeWidth;
            int cellHeightSize = availableHeight / mazeHeight;

            return Math.Min(cellWidthSize, cellHeightSize);
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

        private Rectangle[] CalculateDestRects() {
            Rectangle[] destRects = new Rectangle[_mazeWindows.Length];

            if (CurrentLayout == MazeLayout.FourMazes || CurrentLayout == MazeLayout.ThreeMazes) {
                // For a 2x2 grid layout:
                int mazeWidth = _mazeWindows[0].ScreenWidth;
                int mazeHeight = _mazeWindows[0].ScreenHeight;

                int gridWidth = (mazeWidth * 2) + MAZE_PADDING;
                int gridHeight = (mazeHeight * 2) + MAZE_PADDING;
                int startX = (_windowWidth - gridWidth) / 2;
                int startY = (_windowHeight - gridHeight) / 2;

                for (int i = 0; i < _mazeCount; i++) {
                    int row = i / 2;
                    int col = i % 2;
                    destRects[i] = new Rectangle(
                        startX + (col * (mazeWidth + MAZE_PADDING)),
                        startY + (row * (mazeHeight + MAZE_PADDING)),
                        mazeWidth,
                        mazeHeight
                    );
                }
            }
            else {
                // For a single row layout (OneMaze or TwoMazes):
                int totalWidth = (_mazeWindows[0].ScreenWidth * _mazeWindows.Length) +
                                 (MAZE_PADDING * (_mazeWindows.Length - 1));
                int startX = Math.Abs(_windowWidth - totalWidth) / 2;
                int mazeHeight = _mazeWindows[0].ScreenHeight;

                for (int i = 0; i < _mazeWindows.Length; i++) {
                    destRects[i] = new Rectangle(
                        startX + (i * (_mazeWindows[i].ScreenWidth + MAZE_PADDING)),
                        MAZE_PADDING,
                        _mazeWindows[i].ScreenWidth,
                        mazeHeight
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