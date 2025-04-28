using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components.screens {
    public class ScreenInstruction : AbstractScreen {

        private float _buttonWidth; 
        private const int BACKGROUND_WALL_THICKNESS = 3;
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


        public ScreenInstruction(int parentWindowWidth, int parentWindowHeight, Action onExitAction) 
            : base(parentWindowWidth, parentWindowHeight, onExitAction) { }
        

        public override void Initialize() {
            float maxTextWidth = _buttons.Max(b => Raylib.MeasureTextEx(Raylib.GetFontDefault(), b.label, _fontSize, _textSpacing).X);
            _buttonWidth = maxTextWidth + _vSpace;

            float minButtonWidth = _screenWindow.Width * 0.08f;
            float maxButtonWidth = _screenWindow.Width * 0.30f;
            _buttonWidth = Math.Min(Math.Max(_buttonWidth, minButtonWidth), maxButtonWidth);
            IsInitialized = true;
        }


        public override void Draw(Vector2 mousePos)  {
            base.Draw(mousePos);
        }

        
        protected override float DrawContent(Vector2 mousePos, float currentY) {
            int titleFontSize = DrawTitleAndAdvance(ref currentY, "Instructions", _screenWindow.Width, true);
   
            DrawTitleAndAdvance(ref currentY, "Buttons:", titleFontSize, false);
            foreach (var (label, desc) in _buttons) {
                currentY = ButtonWithDesc(label, desc, currentY);
            }

            DrawTitleAndAdvance(ref currentY, "Tiles:", titleFontSize, false);
            foreach (var (color, desc) in _tiles) {
                currentY = TileWithDesc(color, desc, currentY);
            }

            currentY += _hSpace * 3;

            return currentY; 
                    
        }

        private float TileWithDesc(Color color, string desc, float currentY) {
            Vector2 descSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), desc, _fontSize, _textSpacing);
            float tileX = _screenWindow.X + _vSpace;
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
            float buttonX = _screenWindow.X + _vSpace;
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



    }
}