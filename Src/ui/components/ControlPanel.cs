
using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components {

      

    public class ControlPanel {

        public float ControlPanelHeight => _backButton.Rect.Height + ButtonPadding * 2;

        public event Action? OnReset;

        private Button _backButton;
        private Button _runStopRestartButton;
        private Button _stepButton;
    
        private IGenerator _generator;

        private bool _isRunning = false;

        private readonly int _mazeWidth;
        private readonly int _mazeHeight;
        private int ButtonPadding => Math.Clamp((int)(_mazeWidth * 0.04f), 10, 40);


        public ControlPanel(IGenerator generator, int mazeWidth, int mazeHeight) {
            _generator = generator;
            _mazeWidth = mazeWidth;
            _mazeHeight = mazeHeight;
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
            _backButton.Draw();
            _runStopRestartButton.Draw();
            _stepButton.Draw();
        }        

        public bool IsRunning() => _isRunning;

        private (Button back, Button runStopRe, Button step) InitButtons() {
            int panelY = _mazeHeight + ButtonPadding;
            int fontSize = (int) (_mazeWidth * 0.05f);
            fontSize = Math.Clamp(fontSize, 12, 40);

            int buttonWidth = (int)(_mazeWidth * 0.2f);  // 20% of maze width
            int buttonHeight = fontSize + (2 * ButtonPadding);  // Using your PADDING constant

            // Calculate positions
            float centerX = _mazeWidth / 2;
            float runStopX = centerX - buttonWidth / 2;
            float backX = runStopX - buttonWidth - ButtonPadding;
            float stepX = runStopX + buttonWidth + ButtonPadding;

            Button back = new Button(
                (int)backX, panelY, buttonWidth, buttonHeight,
                "Back", fontSize, () => {
                     _generator.Back();
                    if (_isRunning) {
                        _isRunning = false;
                        _runStopRestartButton.Label = "Run";
                    } 
                 }
            );

            Button runStopRestart = new Button(
                (int)runStopX, panelY, buttonWidth, buttonHeight,
                "Run", fontSize, () => {
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
                (int)stepX, panelY, buttonWidth, buttonHeight,
                "Step", fontSize, () => { 
                    _generator.Step();
                    if (_isRunning) {
                        _isRunning = false;
                        _runStopRestartButton.Label = "Run";
                    }
                 }
            );

            return (back, runStopRestart, step);
        }
    }


}