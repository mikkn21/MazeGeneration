
using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components {

      

    public class ControlPanel {

        public Vector2 Position { get; set; }

        public int Height { get; private set;}
        public int Width { get; private set; }

        public event Action? OnReset;

        private float _fontSize;
        private float _panelY;
        private float _buttonWidth;
        private float  _buttonHeight;
        private Button _backButton;
        private Button _runStopRestartButton;
        private Button _stepButton;
     
        private MazeWindow _mazeWindow;

        public ControlPanel(MazeWindow mazeWindow, int width, int height) {
            _mazeWindow = mazeWindow;
            Height = height;
            Width = width;

            _buttonWidth = Width / 3 * 0.90f;
            _buttonHeight = Height;
            _fontSize = _buttonWidth * 0.25f; // TODO: Clamp this value appropriately
            
            (_backButton, _runStopRestartButton, _stepButton) = InitButtons();
            
        }
        
        public void Update(Vector2 mousePos) {
            UpdateButttonStates(); 
            UpdateButtonsInput(mousePos);
        }

        private void UpdateButttonStates() {
            // Completion state 
            if (_mazeWindow.IsComplete) {
                _runStopRestartButton.Label = "Restart";
                _stepButton.IsEnabled = false;
            } else {
                _stepButton.IsEnabled = true;
            }

            // Running state 
            if (_mazeWindow.IsRunning && !_mazeWindow.IsComplete) {
                // If the "run" button is pressed, disable the other buttons
                _stepButton.IsEnabled = false;
                _backButton.IsEnabled = false;
            } else {
                _backButton.IsEnabled = _mazeWindow.CanUndo;
            }
        }

        private void UpdateButtonsInput(Vector2 mousePos) {
            _backButton.Update(mousePos);
            _runStopRestartButton.Update(mousePos);
            _stepButton.Update(mousePos);
        }

        public void Draw() {
            UpdatePositions();
                        
            _backButton.Draw();
            _runStopRestartButton.Draw();
            _stepButton.Draw();
        }        


        public void UpdatePositions() {
            var positions = CalculatePositions();
            float backX = positions.backX;
            float runStopX = positions.runStopX;
            float stepX = positions.stepX;

            
            float buttonY = (Height - _buttonHeight) / 2;
            _backButton.UpdatePosition(backX + Position.X, buttonY + Position.Y);
            _runStopRestartButton.UpdatePosition(runStopX + Position.X, buttonY + Position.Y);
            _stepButton.UpdatePosition(stepX + Position.X, buttonY + Position.Y);
        }

        private (Button back, Button runStopRe, Button step) InitButtons() {
            var positions = CalculatePositions();
            float backX = positions.backX;
            float runStopX = positions.runStopX;
            float stepX = positions.stepX;

            Button back = new Button(
                backX, _panelY, _buttonWidth, _buttonHeight,
                "Back", _fontSize, () => {
                     _mazeWindow.Back();
                    if (_mazeWindow.IsRunning) {
                        _mazeWindow.IsRunning = false;
                        _runStopRestartButton.Label = "Run";
                    } 
                 }
            );

            Button runStopRestart = new Button(
                runStopX, _panelY, _buttonWidth, _buttonHeight,
                "Run", _fontSize, () => {
                    if (_mazeWindow.IsComplete) {
                        _mazeWindow.restartMaze();
                        _runStopRestartButton.Label = "Run";
                        _mazeWindow.IsRunning = false;
                        _runStopRestartButton.IsEnabled = true;
                        _stepButton.IsEnabled = true;
                        OnReset?.Invoke();
                    }
                    else if (_mazeWindow.IsRunning) {
                        _runStopRestartButton.Label = "Run";
                        _mazeWindow.IsRunning = false;
                    }
                    else {
                        _runStopRestartButton.Label = "Stop";
                        _mazeWindow.IsRunning = true;
                    } 

                 }
            );
                
            Button step = new Button(
                stepX, _panelY, _buttonWidth, _buttonHeight,
                "Step", _fontSize, () => { 
                    _mazeWindow.Step();
                    if (_mazeWindow.IsRunning) {
                        _mazeWindow.IsRunning = false;
                    }
                 }
            );

            return (back, runStopRestart, step);
        }


        private (float backX, float runStopX, float stepX) CalculatePositions() {
            float sectionWidth = Width / 3;

            float leftSectionCenter = sectionWidth / 2;
            float middleSectionCenter = sectionWidth + sectionWidth / 2;
            float rightSectionCenter = 2 * sectionWidth + sectionWidth / 2;           

            // center buttons in its section
            float backX = leftSectionCenter - _buttonWidth / 2;
            float runStopX = middleSectionCenter - _buttonWidth / 2;
            float stepX = rightSectionCenter - _buttonWidth / 2;

            // float centerX = Width / 2;
            // float runStopX = centerX - _buttonWidth / 2;
            // float backX = 0;
            // float stepX = Width - _buttonWidth;

            return (backX, runStopX, stepX);

        }

    }
}