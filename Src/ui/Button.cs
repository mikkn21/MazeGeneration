using Raylib_cs;
using System.Numerics;


namespace MazeGen.ui.button {

    public class Button {

        public Rectangle Rect { get; set; }
        public string Label { get; set; }
        public Color ButtonColor { get; set; }
        public Color TextColor { get; set; }
        public int FontSize { get; set; }
        public Action OnClick { get; set; }


        // Fields for click animation
        private int _clickAnimationFrameCounter = 0;
        private const int _clickAnimationDuration = 20; // Number of frames for the animation
        private const float _clickScale = 0.5f; // Scale factor when the button is clicked

        public Button(Rectangle rect, string label, Color buttonColor, Color textColor, int fontSize, Action onClick) {
            Rect = rect;
            Label = label;
            ButtonColor = buttonColor;
            TextColor = textColor;
            FontSize = fontSize;
            OnClick = onClick;
        }
        public Button(Rectangle rect, string label, Action onClick) {
            Rect = rect;
            Label = label;
            ButtonColor = Color.SkyBlue;
            TextColor = Color.White;
            FontSize = 20;
            OnClick = onClick;
        }

        public void Draw() {

            Rectangle drawRect = Rect;
            if (_clickAnimationFrameCounter > 0) {
                float t = (float)_clickAnimationFrameCounter / _clickAnimationDuration; 
                float scale = _clickScale + (1f - _clickScale) * (1f - t);

                // Scale the button around its center
                float newWidth = Rect.Width * scale;
                float newHeight = Rect.Height * scale;
                float offsetX = (Rect.Width - newWidth) / 2;
                float offsetY = (Rect.Height - newHeight) / 2;
                drawRect = new Rectangle(Rect.X + offsetX, Rect.Y + offsetY, newWidth, newHeight);
            }


            Raylib.DrawRectangleRec(drawRect, ButtonColor);

            // Center the text horizontally and vertically
            int textWidth = Raylib.MeasureText(Label, FontSize);
            int textX = (int)(drawRect.X + (drawRect.Width - textWidth) / 2);
            int textY = (int)(drawRect.Y + (drawRect.Height - FontSize) / 2);
            Raylib.DrawText(Label, textX, textY, FontSize, TextColor);

            // Border around the button
            Raylib.DrawRectangleLinesEx(Rect, 2, Color.Black);

            if (_clickAnimationFrameCounter > 0) {
                _clickAnimationFrameCounter--;
            }
        }

        public void Update(Vector2 mousePos) {
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