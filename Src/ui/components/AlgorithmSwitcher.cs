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

            FontSize = CalculateOptimalFontSize(); 

            (_leftButton, _rightButton) = InitButtons();
            
        }

        private float CalculateOptimalFontSize() {
            string longestName = ""; 
            foreach(string name in _algorithmNames) {
                if (name.Length > longestName.Length) {
                    longestName = name;
                }
            }
            
            float availableWidth = Width - (_buttonWidth * 2) - (Width * 0.2f); // 10% padding each side
    
            float testFontSize = Height * 0.6f;
    
    
            Vector2 textSize;
            do {
                testFontSize -= 1.0f;
                textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), longestName, testFontSize, _textSpacing);
            } while (textSize.X > availableWidth && testFontSize > 8); // Don't go smaller than 8px
    
            return Math.Clamp(testFontSize, Height * 0.3f, Height * 0.6f);
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
            int algIndex = (int)_settingsManager.Settings.Alg;
            if (algIndex < 0 || algIndex >= _algorithmNames.Length) {
                algIndex = 0; 
            }


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