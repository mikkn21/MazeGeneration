using System;
using System.Numerics;
using MazeGen.Algorithms;
using MazeGen.maze;
using MazeGen.maze.wall;
using MazeGen.ui.components.screens;
using Raylib_cs;

namespace MazeGen.ui.components {


    public class MazeLayoutPreview {

        private readonly int _previewWidth;
        private readonly int _previewHeight;

        private readonly int _mazeWidth = 5;
        private readonly int _mazeHeight = 5;
        
        private readonly Dictionary<MazeLayout, RenderTexture2D> _layoutTextures;
        private readonly int _cellSize;
        private int _wallThickness;
        private Random _Random;

        public MazeLayoutPreview(int previewWidth, int previewHeight) {
            _previewWidth = previewWidth;
            _previewHeight = previewHeight;
            _layoutTextures = new Dictionary<MazeLayout, RenderTexture2D>();
            _cellSize = CalculateCellSize();
            _Random = new Random();

            int smallestDim = Math.Min(_mazeWidth, _mazeHeight);
            _wallThickness = Math.Max(1, _cellSize / smallestDim);

            GenerateLayoutPreviews();
        }

        private void GenerateLayoutPreviews() {
            // Create previews for all layouts
            foreach (MazeLayout layout in Enum.GetValues(typeof(MazeLayout))) {
                RenderTexture2D renderTexture = Raylib.LoadRenderTexture(
                    _previewWidth,
                    _previewHeight
                );
                Raylib.SetTextureFilter(renderTexture.Texture, TextureFilter.Point);
                DrawLayoutPreview(renderTexture, layout);
                _layoutTextures[layout] = renderTexture;
            }
        }

        private void DrawLayoutPreview(RenderTexture2D renderTexture, MazeLayout layout) {
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.White);
            
            // Determine the number of mazes for this layout
            int mazeCount = layout switch {
                MazeLayout.OneMaze => 1,
                MazeLayout.TwoMazes => 2,
                MazeLayout.ThreeMazes => 3,
                MazeLayout.FourMazes => 4,
                _ => 1
            };
            
            MazeWindow[] windows = new MazeWindow[mazeCount];
            
            for (int i = 0; i < mazeCount; i++) {
                Maze maze = new Maze(_mazeWidth, _mazeHeight);
                IGenerator generator = new Backtracking(maze);
                
                // Run a few steps to make the maze look partially complete
                int gridSize = _mazeWidth * _mazeHeight;
                int maxStep = _Random.Next(0, gridSize * 2);
                for (int step = 0; step < maxStep; step++) {
                    generator.Step();
                }
                
                windows[i] = new MazeWindow(maze, _cellSize, generator, _wallThickness);
            }

            // Arrange and draw the mazes according to the layout
            DrawLayoutArrangement(layout, windows);
            
            Raylib.EndTextureMode();
        }

        // Calculate a consistent cell size for all layouts based on the smallest available space (4 mazes)
        private int CalculateCellSize() {
            int padding = 2; 

            int availableWidth = (_previewWidth / 2 ) - (3 * padding);
            int availableHeight = (_previewHeight / 2) - (3 * padding);

            int cellWidthSize = availableWidth / _mazeWidth;
            int cellHeightSize = availableHeight / _mazeHeight;
            return Math.Max(1, Math.Min(cellWidthSize, cellHeightSize));
        }

        private void DrawLayoutArrangement(MazeLayout layout, MazeWindow[] windows) {
            int padding = 2;
            int mazeSize = windows[0].Width;

            switch (layout) {
                case MazeLayout.OneMaze:
                    // Center the maze
                    int centerX = (_previewWidth - mazeSize) / 2;
                    int centerY = (_previewHeight - mazeSize) / 2;
                    windows[0].DrawFrame(centerX, centerY);
                    break;
                    
                case MazeLayout.TwoMazes:
                    // Side by side
                    windows[0].DrawFrame(padding, (_previewHeight - mazeSize) / 2);
                    windows[1].DrawFrame(_previewWidth - mazeSize - padding, (_previewHeight - mazeSize) / 2);
                    break;
                    
                case MazeLayout.ThreeMazes:
                    // One on top, two on bottom
                    windows[0].DrawFrame(padding, padding);
                    windows[1].DrawFrame(_previewWidth - mazeSize - padding, padding);
                    windows[2].DrawFrame(padding, _previewHeight - mazeSize - padding);
                    break;
                    
                case MazeLayout.FourMazes:
                    // 2x2 grid
                    windows[0].DrawFrame( padding, padding);
                    windows[1].DrawFrame( _previewWidth - mazeSize - padding, padding);
                    windows[2].DrawFrame( padding, _previewHeight - mazeSize - padding);
                    windows[3].DrawFrame( _previewWidth - mazeSize - padding,
                                         _previewHeight - mazeSize - padding);
                    break;
            }
        }


         public void DrawLayoutOption(MazeLayout layout, float x, float y, float width, float height, bool isSelected) {
            if (!_layoutTextures.ContainsKey(layout))
                return;
                
            Rectangle source = new Rectangle(
                0, 0, 
                _layoutTextures[layout].Texture.Width,
                _layoutTextures[layout].Texture.Height 
            );
            
            Rectangle dest = new Rectangle(x, y, width, height);
            
            
            if (isSelected) {
                Rectangle borderRect = new Rectangle(x - 2, y - 2, width + 4, height + 4);
                Raylib.DrawRectangleLinesEx(borderRect, 2, Color.Green);
                
            }
            
            Raylib.DrawTexturePro(
                _layoutTextures[layout].Texture, 
                source, 
                dest, 
                Vector2.Zero , 
                0f, 
                Color.White
            );
        }

        public void Cleanup() {
            foreach (var texture in _layoutTextures.Values) {
                Raylib.UnloadRenderTexture(texture);
            }
            
            _layoutTextures.Clear();
        }


    }
}