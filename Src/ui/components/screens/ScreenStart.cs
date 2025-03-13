using System.Numerics;
using MazeGen.Algorithms;
using MazeGen.maze;
using Raylib_cs;

namespace MazeGen.ui.components.screens {

    public class ScreenStart : IScreen  {

        public bool IsInitialized { get; private set; } = false;
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly float _vSpace;
        private readonly int _textSpacing = 2;

        private readonly  int _padding = 10;

        private readonly (string Label, Action onClick)[] _menuButtons;
        private readonly Action _onStartClicked;
        private readonly Action _onInstructionsClicked;
        private readonly Action _onSettingsClicked;

        private float _buttonWidth;

        private readonly float _titleFontSize; 
        private readonly float _menuButtonsFontSize;

        // Background maze 
        private Maze _backgroundMaze; 
        private IGenerator _backgroundGenerator;
        private MazeWindow _mazeWindow;
        private const int BACKGROUND_CELL_SIZE = 30;
        private const int BACKGROUND_WALL_THICKNESS = 3;
        private const int FRAMES_PER_STEP = 4;

    
        public ScreenStart(int parentWindowWidth, int parentWindowHeight, Action onStartClicked, Action onInstructionsClicked, Action onSettingsClicked) {
            _windowWidth = parentWindowWidth;
            _windowHeight = parentWindowHeight;
            _onStartClicked = onStartClicked;
            _onInstructionsClicked = onInstructionsClicked;
            _onSettingsClicked = onSettingsClicked;

            _menuButtons = new (string, Action)[] {
                    ("Start", _onStartClicked),
                    ("Settings", _onSettingsClicked),
                    ("Instructions", _onInstructionsClicked)
            };

            _vSpace = Math.Clamp(_windowHeight * 0.02f, 5, 30 );

            _titleFontSize = Math.Clamp(_windowWidth * 0.1f, 15, 140);
            _menuButtonsFontSize = Math.Clamp(_titleFontSize * 0.3f, 12, 40);


            // background init
            int dimx = (int)Math.Ceiling((float)_windowWidth / BACKGROUND_CELL_SIZE);
            int dimy = (int)Math.Ceiling((float)_windowHeight / BACKGROUND_CELL_SIZE);
            _backgroundMaze = new Maze(dimx, dimy);
            _backgroundGenerator = new Backtracking(_backgroundMaze);
            _mazeWindow = new MazeWindow(_backgroundMaze, BACKGROUND_CELL_SIZE, _backgroundGenerator, BACKGROUND_WALL_THICKNESS ,FRAMES_PER_STEP);
        }

        public void Draw(Vector2 mousePos) {
            if (!IsInitialized) {
                Initialize();
            }

            // Title 
            string title = "Maze Generator";

            Vector2 titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, _titleFontSize, _textSpacing);
            
            float rectWidth = titleSize.X + (2 * _padding);
            float rectHeight = titleSize.Y + (2 * _padding);

            Rectangle titleRect = new Rectangle(
                (_windowWidth / 2) - (rectWidth / 2),
                (_windowHeight / 4) - (rectHeight / 2),
                rectWidth,
                rectHeight
            );
            Raylib.DrawRectangleRec(titleRect, Color.SkyBlue);
            Raylib.DrawRectangleLinesEx(titleRect, 2, Color.Black);

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                title,
                new Vector2(titleRect.X + _padding, titleRect.Y + _padding),
                _titleFontSize,
                _textSpacing,
                Color.White
            );


            // Menu buttons
            float currentY = titleRect.Y + rectHeight + _vSpace;
            foreach (var (label, onClick) in _menuButtons) {
                currentY = CreateMenuButton(label, currentY, mousePos, onClick);
            }
        }

        public void Initialize() {
            float maxTextWidth = _menuButtons.Max(b => Raylib.MeasureTextEx(Raylib.GetFontDefault(), b.Label, _menuButtonsFontSize, _textSpacing).X);
            _buttonWidth = maxTextWidth + _vSpace;

            float minButtonWidth = _windowWidth * 0.08f;
            float maxButtonWidth = _windowWidth * 0.30f;
            _buttonWidth = Math.Min(Math.Max(_buttonWidth, minButtonWidth), maxButtonWidth);
            IsInitialized = true;
        }



        private float CreateMenuButton(string buttonText, float currentY, Vector2 mousePos, Action onClick) {
            Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), buttonText, _menuButtonsFontSize, _textSpacing);
            float buttonHeight = textSize.Y + _vSpace;

            float buttonX = (_windowWidth / 2) - (_buttonWidth / 2);
            float buttonY = currentY;

             Button btn = new Button(
                buttonX,
                buttonY,
                _buttonWidth,
                buttonHeight,
                buttonText,
                _menuButtonsFontSize,
                onClick
            );

            btn.Update(mousePos);
            btn.Draw();

            return currentY + buttonHeight + _vSpace;
        }

         public void DrawBackgroundMaze() {
            

            _mazeWindow.DrawFrame();
            if (_backgroundGenerator.IsComplete) {
                _backgroundGenerator.Restart();
            }
        }




    }
}