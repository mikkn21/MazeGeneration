
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
     
        private IGenerator _generator;

        private bool _isRunning = false;

        public ControlPanel(IGenerator generator, int width, int height) {
            _generator = generator;
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
            if (_generator.IsComplete) {
                _runStopRestartButton.Label = "Restart";
                _stepButton.IsEnabled = false;
            } else {
                _stepButton.IsEnabled = true;
            }

            // Running state 
            if (_isRunning && !_generator.IsComplete)
            {
                // If the "run" button is pressed, disable the other buttons
                _stepButton.IsEnabled = false;
                _backButton.IsEnabled = false;
            } else {
                _backButton.IsEnabled = _generator.CanUndo;
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

        public bool IsRunning() => _isRunning;


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
                     _generator.Back();
                    if (_isRunning) {
                        _isRunning = false;
                        _runStopRestartButton.Label = "Run";
                    } 
                 }
            );

            Button runStopRestart = new Button(
                runStopX, _panelY, _buttonWidth, _buttonHeight,
                "Run", _fontSize, () => {
                    if (_generator.IsComplete) {
                        _generator.Restart();
                        _runStopRestartButton.Label = "Run";
                        _isRunning = false;
                        _runStopRestartButton.IsEnabled = true;
                        _stepButton.IsEnabled = true;
                        OnReset?.Invoke();
                    }
                    else if (_isRunning) {
                        _runStopRestartButton.Label = "Run";
                        _isRunning = false;
                    }
                    else {
                        _runStopRestartButton.Label = "Stop";
                        _isRunning = true;
                    } 

                 }
            );
                
            Button step = new Button(
                stepX, _panelY, _buttonWidth, _buttonHeight,
                "Step", _fontSize, () => { 
                    _generator.Step();
                    if (_isRunning) {
                        _isRunning = false;
                        _runStopRestartButton.Label = "Run";
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