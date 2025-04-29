using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components {

    public class AlgorithmSwitcher {

        public Vector2 Position { get; set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        public float FontSize { get; private set; }
        
        private float _buttonWidth;
        private float _buttonHeight;
        private readonly Button _leftButton;
        private readonly Button _rightButton;
        private readonly int _textSpacing = 2;


        private readonly MazeSettingsModel _settingsManager;
        private MazeWindow _mazeWindow;

        private static readonly string[] _algorithmNames = new[] { 
            "Backtracking", 
            // "Prim's", 
            // "Kruskal's"
        };
        
        public AlgorithmSwitcher(MazeWindow mazeWindow, int width, int height, MazeSettingsModel settingsManager) {
            _mazeWindow = mazeWindow;
            Width = width;
            Height = height;
            _settingsManager = settingsManager;

            _buttonHeight = Height * 0.8f;
            _buttonWidth = Height; // Square button 

            (_leftButton, _rightButton) = InitButtons();
            
        }

        private (Button left, Button right) InitButtons() {
            var positions = CalculatePositions();
            Button left = new Button(
                positions.leftX, 0, _buttonWidth, _buttonHeight,
                "<", FontSize, () => {
                    if (!_mazeWindow.IsRunning) {
                        int currentIndex = (int)_settingsManager.Settings.Alg;
                        int newIndex = (currentIndex - 1 + _algorithmNames.Length) % _algorithmNames.Length;
                        _settingsManager.UpdateAlgorithm((AlgorithmType)newIndex);
                    }
                }
            );

            Button right = new Button(
                positions.rightX, 0, _buttonWidth, _buttonHeight,
                ">", FontSize, () => {
                    if (!_mazeWindow.IsRunning) {
                        int currentIndex = (int)_settingsManager.Settings.Alg;
                        int newIndex = (currentIndex + 1) % _algorithmNames.Length;
                        _settingsManager.UpdateAlgorithm((AlgorithmType)newIndex);
                    }
                }
            );

            return (left, right);

        }

        private (float leftX, float rightX) CalculatePositions() {
            float padding = Width * 0.05f;
            float leftX = padding;
            float rightX = Width - _buttonWidth - padding;
            
            return (leftX, rightX);
        }

        public void Update(Vector2 mousePos) {
            _leftButton.Update(mousePos);
            _rightButton.Update(mousePos);
        }
        
        public void UpdatePositions() {
            var positions = CalculatePositions();
            float buttonY = (Height - _buttonHeight) / 2;
            
            _leftButton.UpdatePosition(positions.leftX + Position.X, buttonY + Position.Y);
            _rightButton.UpdatePosition(positions.rightX + Position.X, buttonY + Position.Y);
        }

        public void Draw() {
            UpdatePositions();
            _leftButton.Draw();
            _rightButton.Draw();
            DrawAlgTitle();
        }

        private void DrawAlgTitle() {
            string currentAlgorithm = _algorithmNames[(int)_settingsManager.Settings.Alg];
            Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), currentAlgorithm, FontSize, _textSpacing);
            
            float textX = Position.X + (Width - textSize.X) / 2;
            float textY = Position.Y + (Height - textSize.Y) / 2;
            
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                currentAlgorithm,
                new Vector2(textX, textY),
                FontSize,
                _textSpacing,
                Color.Black
            );
        }


        

        


    }
}