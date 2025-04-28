
using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components.screens {

    public abstract class AbstractScreen : IScreen {

        public bool IsInitialized { get; protected set; } = false;


        protected readonly float _windowWidth;
        protected readonly float _windowHeight;
        protected readonly Rectangle _screenWindow;
        protected readonly float _vSpace;
        protected readonly float _hSpace;
        protected readonly float _fontSize;
        protected readonly int _textSpacing = 2;
        protected readonly Action _onExitAction; 

        // Scrolling 
        protected const int SCROLLSPEED = 20; 
        protected bool _isDraggingScrollbar = false;
        protected float _scrollbarDragOffset = 0;
        protected float _scrollY = 0;
        protected float _totalContentHeight = 0;
        protected virtual bool UseScrolling => false;

        protected AbstractScreen(int parentWindowWidth, int parentWindowHeight, Action onExitAction, float windowScale = 0.9f) {
            _windowWidth = parentWindowWidth * windowScale;
            _windowHeight = parentWindowHeight * windowScale;
            _onExitAction = onExitAction;
            
            _screenWindow = new Rectangle(
                (parentWindowWidth - _windowWidth) / 2,
                (parentWindowHeight - _windowHeight) / 2,
                _windowWidth,
                _windowHeight
            );
            
            _vSpace = Math.Clamp(_screenWindow.Width * 0.02f, 5, 30);
            _hSpace = Math.Clamp(_screenWindow.Height * 0.02f, 5, 30);
            _fontSize = Math.Clamp(_screenWindow.Width * 0.03f, 12, 30);
        }

        public abstract void Initialize();

        public virtual void Draw(Vector2 mousePos) {
            if (!IsInitialized) {
                Initialize();
            }

            // Draw transparrent background window
            Color transColor = new Color(200, 200, 200, 240);
            Raylib.DrawRectangleRec(_screenWindow, transColor);
            Raylib.DrawRectangleLinesEx(_screenWindow, 2, Color.Black);

            float wheel = Raylib.GetMouseWheelMove();
            _scrollY += wheel * SCROLLSPEED;

            Raylib.BeginScissorMode(
                (int)_screenWindow.X,
                (int)_screenWindow.Y,
                (int)_screenWindow.Width,
                (int)_screenWindow.Height
            );

            float currentY = _screenWindow.Y + _hSpace + _scrollY;
            float startY = currentY;

            
            currentY = DrawContent(mousePos, currentY);


            _totalContentHeight = currentY - startY;

            Raylib.EndScissorMode();

            Button exitButton = ExitButton();
            exitButton.Update(mousePos);
            exitButton.Draw();

            ScrollBar(mousePos);
        }

        // Template method for child classes to implement for Draw        
        protected abstract float DrawContent(Vector2 mousePos, float startY);

        public virtual void Cleanup() { }

        protected int DrawTitleAndAdvance(ref float currentY, string title, float scaleElement, bool isWindowTitle = false) {
            var (height, fontSize) = DrawTitle(currentY, title, scaleElement, isWindowTitle);
            currentY += height + _hSpace;
            return fontSize;
        }

        protected (float height, int fontSize) DrawTitle(float currentY, string title, float scaleElement, bool isWindowTitle = false) {
            int fontSize = isWindowTitle ? 
                Math.Clamp((int)(scaleElement * 0.05f), 20, 70) : 
                Math.Clamp((int)(scaleElement * 0.70), 12, 50);
                
            Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, fontSize, _textSpacing);
            
            float textX = isWindowTitle ?
                _screenWindow.X + (_screenWindow.Width - textSize.X) / 2 :
                _screenWindow.X + _vSpace;
                
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                title,
                new Vector2(textX, currentY),
                fontSize,
                _textSpacing,
                isWindowTitle ? Color.White : Color.Black
            );
            
            if (isWindowTitle) {
                Vector2 startPos = new Vector2(textX, currentY + textSize.Y);
                Vector2 endPos = new Vector2(textX + textSize.X, currentY + textSize.Y);
                Raylib.DrawLineEx(startPos, endPos, 2, Color.Black);
            }
            
            return (textSize.Y, fontSize);
        }

        protected Button ExitButton() {
            string text = "Exit";
            
            float targetWidth = _screenWindow.Width * 0.1f;
            float targetHeight = _screenWindow.Height * 0.08f;
            float initialFontSize = _fontSize * 0.6f;
            Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, initialFontSize, _textSpacing);
            
            float availableWidth = targetWidth - _hSpace;
            float availableHeight = targetHeight - _vSpace;
            float widthRatio = availableWidth / textSize.X;
            float heightRatio = availableHeight / textSize.Y;
            float scaleFactor = Math.Min(widthRatio, heightRatio);
            float scaledFontSize = Math.Clamp(initialFontSize * scaleFactor, 10, _fontSize);
            
            textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, scaledFontSize, _textSpacing);
            float width = textSize.X + _hSpace;
            float height = textSize.Y + _vSpace;
            
            return new Button(
                _screenWindow.X + _screenWindow.Width - _vSpace - width,
                _screenWindow.Y + _hSpace,  // No scrolling for exit button
                width,
                height,
                text,
                scaledFontSize,
                _onExitAction
            );
        }

        protected void ScrollBar(Vector2 mousePos) {
            float maxScroll = Math.Max(0, _totalContentHeight - _screenWindow.Height);
            _scrollY = Math.Clamp(_scrollY, -maxScroll, 0);
            
            if (_totalContentHeight > _screenWindow.Height) {
                float scrollbarWidth = 8;
                float visibleRatio = _screenWindow.Height / _totalContentHeight;
                float scrollbarHeight = _screenWindow.Height * visibleRatio;
                
                float scrollProgress = -_scrollY / maxScroll;
                float scrollbarY = _screenWindow.Y + (_screenWindow.Height - scrollbarHeight) * scrollProgress;
                
                Rectangle scrollbar = new Rectangle(
                    _screenWindow.X + _screenWindow.Width - scrollbarWidth - 4,
                    scrollbarY,
                    scrollbarWidth,
                    scrollbarHeight
                );
                
                HandleScrollbarInteraction(mousePos, scrollbar, scrollbarHeight, maxScroll);
                
                Color scrollbarColor = _isDraggingScrollbar ?
                    new Color(100, 100, 100, 220) :
                    new Color(130, 130, 130, 180);
                    
                Raylib.DrawRectangleRec(scrollbar, scrollbarColor);
            }
        }

         private void HandleScrollbarInteraction(Vector2 mousePos, Rectangle scrollbar, float scrollbarHeight, float maxScroll) {
            if (Raylib.CheckCollisionPointRec(mousePos, scrollbar) && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                _isDraggingScrollbar = true;
                _scrollbarDragOffset = mousePos.Y - scrollbar.Y;
            }
            
            if (_isDraggingScrollbar) {
                if (Raylib.IsMouseButtonDown(MouseButton.Left)) {
                    float newScrollbarY = mousePos.Y - _scrollbarDragOffset;
                    float newScrollProgress = (newScrollbarY - _screenWindow.Y) / (_screenWindow.Height - scrollbarHeight);
                    newScrollProgress = Math.Clamp(newScrollProgress, 0, 1);
                    _scrollY = -newScrollProgress * maxScroll;
                } else {
                    _isDraggingScrollbar = false;
                }
            }
        }


    }
}