using Raylib_cs;
using System.Numerics;


namespace MazeGen.ui.Components {

    public class Button {

        public Rectangle Rect { get; private set; }


        public string Label {get; set;}

        public float Width {get; private set;}
        public float Height {get; private set;}
        public Color ButtonColor { get; set; }
        public Color TextColor { get; set; }
        public bool IsEnabled { get; set; } = true;
        public Action OnClick { get; set; }

        private float _fontSize {get; set; }

        // Flag to indicate if we need to (re)calculate the button dimensions
        private bool _needsMeasurement = true; 
        private const int PADDING = 10; // padding for reactangle
        private const int TEXTSPACING = 2;

        // Fields for click animation
        private int _clickAnimationFrameCounter = 0;
        private const int _clickAnimationDuration = 20; // Number of frames for the animation
        private const float _clickScale = 0.5f; // Scale factor when the button is clicked


        public Button(float x, float y, float width, float height, string label, float fontSize, Action onClick) {
            Label = label;
            ButtonColor = Color.SkyBlue;
            TextColor = Color.White;
            _fontSize = fontSize;
            OnClick = onClick;

            Width = width;
            Height = height;
            Rect = new Rectangle(x, y, Width, Height);
        }


        public void Draw() {
            Rectangle drawRect = Rect;
            if (_clickAnimationFrameCounter > 0) {
                float t = (float)_clickAnimationFrameCounter / _clickAnimationDuration; 
                float scale = _clickScale + (1f - _clickScale) * (1f - t);

                // Scale the button around its center
                float newWidth = Width * scale;
                float newHeight = Height * scale;
                float offsetX = (Width - newWidth) / 2;
                float offsetY = (Height - newHeight) / 2;
                drawRect = new Rectangle(Rect.X + offsetX, Rect.Y + offsetY, newWidth, newHeight);
            }


            Raylib.DrawRectangleRec(drawRect, ButtonColor);

            // Center the text horizontally and vertically
            Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), Label, _fontSize, TEXTSPACING);
            float textX = drawRect.X + (drawRect.Width - textSize.X) / 2;
            float textY = drawRect.Y + (drawRect.Height - textSize.Y) / 2;
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                Label,
                new Vector2(textX, textY),
                _fontSize,
                TEXTSPACING,
                TextColor
            );

            // Border around the button
            Raylib.DrawRectangleLinesEx(drawRect, 2, Color.Black);

            if (_clickAnimationFrameCounter > 0) {
                _clickAnimationFrameCounter--;
            }
        }

        public void Update(Vector2 mousePos) {
            if (!IsEnabled) {
                TextColor = Color.Gray; 
                return;
            }
            
            if (Raylib.CheckCollisionPointRec(mousePos, Rect)) {
                TextColor = Color.Yellow;
                if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                    OnClick?.Invoke();
                    _clickAnimationFrameCounter = _clickAnimationDuration;
                }
            }
            else {
                TextColor = Color.White;
            }
        }
    }
}