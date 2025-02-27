using Raylib_cs;
using System.Numerics;


namespace MazeGen.ui.Components {

    public class Button {

        public Rectangle Rect { get; private set; }


        public string Label {get; set;}

        public int Width {get; private set;}
        public int Height {get; private set;}
        public Color ButtonColor { get; set; }
        public Color TextColor { get; set; }
        public bool IsEnabled { get; set; } = true;
        public Action OnClick { get; set; }

        private int _fontSize {get; set; }

        // Flag to indicate if we need to (re)calculate the button dimensions
        private bool _needsMeasurement = true; 
        private const int PADDING = 10; // padding for reactangle

        // Fields for click animation
        private int _clickAnimationFrameCounter = 0;
        private const int _clickAnimationDuration = 20; // Number of frames for the animation
        private const float _clickScale = 0.5f; // Scale factor when the button is clicked


        public Button(int x, int y, int width, int height, string label, int fontSize, Action onClick) {
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
            int textWidth = Raylib.MeasureText(Label, _fontSize);
            int textX = (int)(drawRect.X + (drawRect.Width - textWidth) / 2);
            int textY = (int)(drawRect.Y + (drawRect.Height - _fontSize) / 2);
            Raylib.DrawText(Label, textX, textY, _fontSize, TextColor);

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