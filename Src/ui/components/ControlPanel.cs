
using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.Components {

      

    public class ControlPanel {


        public int ControlPanelHeight => BUTTON_HEIGHT + BUTTON_PADDING * 2;

        public event Action? OnReset;

        private Button _backButton;
        private Button _runStopRestartButton;
        private Button _stepButton;
    
        private IGenerator _generator;

        private bool _isRunning = false;

        private readonly int _mazeWidth;
        private readonly int _mazeHeight;

        // Button properties
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_WIDTH = 150;
        private const int BUTTON_PADDING = 10;

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

        private (Button back, Button runReset, Button step) InitButtons()
        {
            int horizontalCenterPos = (_mazeWidth - BUTTON_WIDTH) / 2;

            int panelY = _mazeHeight + BUTTON_PADDING;

            Button back = new Button(
                new Rectangle(
                    horizontalCenterPos - BUTTON_WIDTH - BUTTON_PADDING,
                    panelY,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT),
                "Back",
                () =>
                {
                    _generator.Back();
                    if (_isRunning) {
                        _isRunning = false;
                        _runStopRestartButton.Label = "Run";
                    }
                }
            );

            
            Button runStopRestart = new Button(
                new Rectangle(
                    horizontalCenterPos,
                    panelY,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT),
                "Run",
                () =>
                {
                    if (_generator.IsComplete) {
                        _generator.Restart();
                        _runStopRestartButton.Label = "Run";
                        _isRunning = false;
                        _runStopRestartButton.IsEnabled = true;
                        _stepButton.IsEnabled = true;
                        OnReset?.Invoke();
                    }
                    else if (_isRunning) {
                        _runStopRestartButton.Label = "run";
                        _isRunning = false;
                    }
                    else {
                        _runStopRestartButton.Label = "stop";
                        _isRunning = true;
                    }
                }
            );

            Button step = new Button(
                new Rectangle(
                    horizontalCenterPos + BUTTON_WIDTH + BUTTON_PADDING,
                    panelY,
                    BUTTON_WIDTH,
                    BUTTON_HEIGHT),
                "Step",
                () => { _generator.Step(); }
            );

            return (back, runStopRestart, step);
        }
    }


}