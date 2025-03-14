using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components.screens {

    public class ScreenSettings : IScreen {

        public bool IsInitialized { get; private set; } = false;
        private float _windowWidth;
        private float _windowHeight;
        private Action _onExitAction;
        private Rectangle _settingsWindow;
        private object _vSpace;
        private object _hSpace;
        private object _fontSize;

        public ScreenSettings(int parentWindowWidth, int parentWindowHeight, Action onExitAction) {
            _windowWidth = parentWindowWidth * 0.90f;
            _windowHeight = parentWindowHeight * 0.90f;

            _onExitAction = onExitAction;

            _settingsWindow = new Rectangle(
                (parentWindowWidth - _windowWidth) / 2,
                (parentWindowHeight - _windowHeight) / 2,
                _windowWidth,
                _windowHeight
            );

            _vSpace = Math.Clamp(_settingsWindow.Width* 0.02f, 5, 30); // TODO: Check clamp values
            _hSpace = Math.Clamp(_settingsWindow.Height* 0.02f, 5, 30); // TODO: Check clamp values
            _fontSize = Math.Clamp(_settingsWindow.Width * 0.03f, 12, 30);            
        }


        public void Initialize(){
            IsInitialized = true;
        }

        public void Draw(Vector2 mousePos) {
            if (!IsInitialized) {
                Initialize();
            }

            // Transparrent color for the window 
            Color transColor = new Color(200, 200, 200, 240);
            Raylib.DrawRectangleRec(_settingsWindow, transColor);
            Raylib.DrawRectangleLinesEx(_settingsWindow, 2, Color.Black);




        }


    }
}