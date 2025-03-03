using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components {
    public class ScreenInstruction {

        public bool IsInitialized { get; private set; } = false;
        
        private float _instructionScrollY = 0;
        private float _totalInstructionContentHeight = 0;

        private readonly float _windowWidth;
        private readonly float _windowHeight;
        private readonly Rectangle _instructionWindow;
        private float _buttonWidth;
        private readonly float _vSpace;
        private readonly float _hSpace;
        private readonly int _textSpacing = 2;
        private readonly Action _onExitAction;
        private const int BACKGROUND_WALL_THICKNESS = 3;
        private readonly float _fontSize;
        private const int SCROLLSPEED = 20;


        private readonly (string label, string desc)[] _buttons = new (string label, string desc)[] {
                ("Step", "Performs one step of the maze generation algorithm"),
                ("Run", "Continuously runs the algorithm until completion or stop"),
                ("Stop", "Stops the algorithm if it is currently running"),
                ("Restart", "Restarts the algorithm with a new seed"),
                ("Back", "Undoes the last step of the algorithm"),
        };

        private readonly (Color color, String desc)[] _tiles = new (Color color, String desc)[] {
                (Color.Gray, "The algorithm has not visited this tile"),
                (Color.LightGray, "The algorithm has visited this tile"),
                (Color.White, "The algorithm has selected this tile for the final maze"),
                (Color.Red, "The current tile the algorithm is working on")
        };

        
        public ScreenInstruction(int parentWindowWidth, int parentWindowHeight, Action onExitAction) {
            _windowWidth = parentWindowWidth * 0.9f;
            _windowHeight = parentWindowHeight * 0.9f;
            _onExitAction = onExitAction;

            _instructionWindow = new Rectangle(
                (parentWindowWidth - _windowWidth) / 2,
                (parentWindowHeight - _windowHeight) / 2,
                _windowWidth,
                _windowHeight
            );

            _vSpace = Math.Clamp(_instructionWindow.Width * 0.02f, 5, 30); // TODO: Check clamp values
            _hSpace = Math.Clamp(_instructionWindow.Height * 0.02f, 5, 30); // TODO: Check clamp values

            _fontSize = Math.Clamp(_instructionWindow.Width * 0.03f, 12, 30); // font size for buttons and descriptions

        }

        public void Draw(Vector2 mousePos)  {
            if (!IsInitialized) {
                // Optionally, initialize automatically if not already done
                Initialize();
            }


            // Transparrent color for the window 
            Color transColor = new Color(200, 200, 200, 240);
            Raylib.DrawRectangleRec(_instructionWindow, transColor);
            Raylib.DrawRectangleLinesEx(_instructionWindow, 2, Color.Black);


            float wheel = Raylib.GetMouseWheelMove();
            _instructionScrollY += wheel * SCROLLSPEED; 

            Raylib.BeginScissorMode(
                (int)_instructionWindow.X,
                (int)_instructionWindow.Y,
                (int)_instructionWindow.Width,
                (int)_instructionWindow.Height
            );

            float currentY = _instructionWindow.Y + _hSpace + _instructionScrollY;
            float startY = currentY;

            // Draw title
            string title = "Instructions";
            int titleFontSize = Math.Clamp((int)(_instructionWindow.Width * 0.05f), 20, 70);   
            Vector2 titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, titleFontSize, _textSpacing);
            float titleX = _instructionWindow.X + (_instructionWindow.Width - titleSize.X) / 2;
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                title,
                new Vector2(titleX, currentY),
                titleFontSize,
                _textSpacing,
                Color.White
            );

            // underline title 
            Vector2 startPos = new Vector2(titleX, currentY + titleSize.Y);
            Vector2 endPos = new Vector2(titleX + titleSize.X, currentY + titleSize.Y);
            Raylib.DrawLineEx(startPos, endPos, 2, Color.Black);

            currentY += titleSize.Y + _hSpace;

            // Draw instruction button section
            string buttonSectionTitle = "Buttons:";
            int sectionFontSize = Math.Clamp((int)(titleFontSize * 0.70f), 12, 50); // TODO: Maybe have this scale with windowWidth of the section titel
            Vector2 buttonSectionSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), buttonSectionTitle, sectionFontSize, _textSpacing);

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                buttonSectionTitle,
                new Vector2(_instructionWindow.X + _vSpace, currentY),
                sectionFontSize,
                _textSpacing,
                Color.Black

            );
            currentY += buttonSectionSize.Y + _hSpace;  

            foreach (var (label, desc) in _buttons) {
                currentY = ButtonWithDesc(label, desc, currentY);
            }


            // Draw instruction tile section 
            string tileSectionTitle = "Tiles:";
            Vector2 tileSectionSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), tileSectionTitle, sectionFontSize, _textSpacing);

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                tileSectionTitle,
                new Vector2(_instructionWindow.X + _vSpace, currentY),
                sectionFontSize,
                _textSpacing,
                Color.Black
            );
            currentY += tileSectionSize.Y + _hSpace;
            
            foreach (var (color, desc) in _tiles) {
                currentY = TileWithDesc(color, desc, currentY);
            }

            currentY += _hSpace * 3; // Add some padding at the bottom

            Button exitButton = ExitButton();
            exitButton.Update(mousePos);
            exitButton.Draw();

            _totalInstructionContentHeight = currentY - startY;
            Raylib.EndScissorMode();

            // Draw scrollbar 
            ScrollBar();
 

        }

        public void Initialize() {
            float maxTextWidth = _buttons.Max(b => Raylib.MeasureTextEx(Raylib.GetFontDefault(), b.label, _fontSize, _textSpacing).X);
            _buttonWidth = maxTextWidth + _vSpace;

            float minButtonWidth = _instructionWindow.Width * 0.08f;
            float maxButtonWidth = _instructionWindow.Width * 0.30f;
            _buttonWidth = Math.Min(Math.Max(_buttonWidth, minButtonWidth), maxButtonWidth);
            IsInitialized = true;
        }

        private float TileWithDesc(Color color, string desc, float currentY) {
            Vector2 descSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), desc, _fontSize, _textSpacing);
            float tileX = _instructionWindow.X + _vSpace;
            float tileSize = descSize.Y + _vSpace;

            float textStartX = tileX + _buttonWidth + _hSpace;
            float textStartY = currentY + (tileSize - _fontSize) / 2;
            Rectangle rect = new Rectangle(
                    tileX,
                    currentY,
                    tileSize,
                    tileSize
            );
            if (color.Equals(Color.Red))
            {
                Raylib.DrawRectangleRec(rect, Color.Blank);
                float centerX = tileX + tileSize / 2;
                float centerY = currentY + tileSize / 2;
                float radius = tileSize / 4;
                Raylib.DrawCircle((int)centerX, (int)centerY, radius, Color.Red);
            }
            else {
                Raylib.DrawRectangleRec(rect, color);
            }


            Raylib.DrawRectangleLinesEx(rect, BACKGROUND_WALL_THICKNESS, Color.Black);

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                desc,
                new Vector2(textStartX, textStartY),
                _fontSize,
                _textSpacing,
                Color.Black
            );

            return currentY + tileSize + _hSpace;
        }


        private float ButtonWithDesc(string buttonText, string description, float currentY) {
            Vector2 buttonTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), buttonText, _fontSize, _textSpacing);

            float buttonHeight = buttonTextSize.Y + _vSpace;  // half vspace padding on each side
            float buttonX = _instructionWindow.X + _vSpace;
            float buttonY = currentY;

            float descStartX = buttonX + _buttonWidth + _hSpace;
            float descStartY = buttonY + (buttonHeight - _fontSize) / 2;

            Button button = new Button(
                buttonX, buttonY, _buttonWidth, buttonHeight,
                buttonText,
                _fontSize,
                () => { }
            );
            button.Draw();

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                description,
                new Vector2(descStartX, descStartY),
                _fontSize,
                _textSpacing,
                Color.Black
            );

            return currentY + buttonHeight + _hSpace;
        }


        private Button ExitButton() {
            float exitButtonScaleFactor = 0.6f;
            string text = "Exit";

            Vector2 buttonTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, _fontSize * exitButtonScaleFactor, _textSpacing);

            float exitButtonHeight = (buttonTextSize.Y + _vSpace) * exitButtonScaleFactor;
            float exitButtonWidth = _buttonWidth * exitButtonScaleFactor;

            return new Button(
                _instructionWindow.X + _instructionWindow.Width - _vSpace - exitButtonWidth,
                _instructionWindow.Y + _hSpace+ _instructionScrollY,
                exitButtonWidth,
                exitButtonHeight,
                text,
                _fontSize * exitButtonScaleFactor,
                _onExitAction
            );
        }

        private void ScrollBar() {
            float maxScroll = Math.Max(0, _totalInstructionContentHeight - _instructionWindow.Height);
            _instructionScrollY = Math.Clamp(_instructionScrollY, -maxScroll, 0);

            if (_totalInstructionContentHeight > _instructionWindow.Height) {
                float scrollbarWidth = 8;
                float visibleRatio = _instructionWindow.Height / _totalInstructionContentHeight;
                float scrollbarHeight = _instructionWindow.Height * visibleRatio;

                float scrollProgress = -_instructionScrollY / maxScroll;
                float scrollbarY = _instructionWindow.Y + (_instructionWindow.Height - scrollbarHeight) * scrollProgress;

                Rectangle scrollbar = new Rectangle(
                    _instructionWindow.X + _instructionWindow.Width - scrollbarWidth - 4,
                    scrollbarY,
                    scrollbarWidth,
                    scrollbarHeight
                );

                Raylib.DrawRectangleRec(scrollbar, new Color(100, 100, 100, 180));
            }
        }
    }

}