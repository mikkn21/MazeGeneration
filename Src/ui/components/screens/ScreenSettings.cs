using System.Numerics;
using System.Runtime.InteropServices;
using MazeGen.maze;
using Raylib_cs;

namespace MazeGen.ui.components.screens {

    public class ScreenSettings : IScreen {

        public bool IsInitialized { get; private set; } = false;
        // public MazeSettings CurrentSettings;

        private float _windowWidth;
        private float _windowHeight;
        private Action _onExitAction;
        private Rectangle _settingsWindow;
        private float _vSpace;
        private float _hSpace;
        private float _fontSize;
        private readonly int _textSpacing = 2;

        private MazeLayoutPreview? _previewManager; 
        private MazeLayout _selectedLayout;

        MazeSettingsModel _settingsManager;
        
        
        
        public ScreenSettings(int parentWindowWidth, int parentWindowHeight, Action onExitAction, MazeSettingsModel settingsManager) {
            _windowWidth = parentWindowWidth * 0.90f;
            _windowHeight = parentWindowHeight * 0.90f;

            _onExitAction = onExitAction;

            _settingsWindow = new Rectangle(
                (parentWindowWidth - _windowWidth) / 2,
                (parentWindowHeight - _windowHeight) / 2,
                _windowWidth,
                _windowHeight
            );

            _settingsManager = settingsManager;
            _selectedLayout = _settingsManager.Settings.Layout;

            _vSpace = Math.Clamp(_settingsWindow.Width* 0.02f, 5, 30); 
            _hSpace = Math.Clamp(_settingsWindow.Height* 0.02f, 5, 30); 
            _fontSize = Math.Clamp(_settingsWindow.Width * 0.03f, 12, 30);    

        }

        public void Initialize(){
            IsInitialized = true;
            _previewManager = new MazeLayoutPreview(150, 150);        
        }

        public void Draw(Vector2 mousePos) {
            if (!IsInitialized) {
                Initialize();
            }

            // Transparrent color for the window 
            Color transColor = new Color(200, 200, 200, 240);
            Raylib.DrawRectangleRec(_settingsWindow, transColor);
            Raylib.DrawRectangleLinesEx(_settingsWindow, 2, Color.Black);

            float currentY = _settingsWindow.Y + _hSpace; //+ _instructionScrollY;
            float startY = currentY;

            // Draw title
            string title = "Settings";
            int titleFontSize = Math.Clamp((int)(_settingsWindow.Width * 0.05f), 20, 70);   
            Vector2 titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, titleFontSize, _textSpacing);
            float titleX = _settingsWindow.X + (_settingsWindow.Width - titleSize.X) / 2;
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

            float LayoutSectionHeight = DrawLayoutSection(currentY, mousePos, titleFontSize);

            // currentY += LayoutSectionHeight + _hSpace;

            Button exitButton = ExitButton();
            exitButton.Update(mousePos);
            exitButton.Draw();

        }

        // Draw Layout section 
        private float DrawLayoutSection(float currentY, Vector2 mousePos, int titleFontSize) {

            string layoutSectionTitle = "Layout:";
            int sectionFontSize = Math.Clamp((int)(titleFontSize * 0.70f), 12, 50); 
            Vector2 sectionTitleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), layoutSectionTitle, sectionFontSize, _textSpacing);

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                layoutSectionTitle,
                new Vector2(_settingsWindow.X + _vSpace, currentY),
                sectionFontSize,
                _textSpacing,
                Color.Black

            );
            currentY += sectionTitleSize.Y + _hSpace;

            
            string[] layoutNames = Enum.GetNames(typeof(MazeLayout));
            string longestName = layoutNames.OrderByDescending(name => name.Length).First();

            float labelFontSize = Math.Clamp(sectionFontSize * 0.45f, 8, 20); // Labels are 45% of the section font size

            Vector2 longestTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), longestName, labelFontSize, _textSpacing);

            float previewWidth = longestTextSize.X * 1.1f;
            float availableWidth = _settingsWindow.Width - (2 * _vSpace);
        
            float totalPreviewsWidth = 4 * previewWidth;
            float remainingSpace = availableWidth - totalPreviewsWidth;
            float spacing = Math.Clamp(remainingSpace / 3, 10, 35); 

            
            // Center the layout options
            float totalWidth = (4 * previewWidth) + (3 * spacing);
            float startX = _settingsWindow.X + _vSpace +  (_settingsWindow.Width - totalWidth) / 2;
            
            for (int i = 0; i < 4; i++) {
                MazeLayout layout = (MazeLayout)i;
                float x = startX + i * ( previewWidth +  spacing);
                bool isSelected = layout == _selectedLayout;

                _previewManager!.DrawLayoutOption(layout, x, currentY, previewWidth, previewWidth, isSelected);

                string layoutName = layout.ToString();
                Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), layoutName, labelFontSize, _textSpacing);
                
                float textX = x + (previewWidth - textSize.X) / 2;
                float textY = currentY + previewWidth + _hSpace;

                Raylib.DrawTextEx(
                    Raylib.GetFontDefault(),
                    layoutName,
                    new Vector2(textX, textY),
                    labelFontSize,
                    _textSpacing,
                    Color.Black
                );

                Rectangle layoutRect = new Rectangle(x, currentY, previewWidth, previewWidth);
                if (Raylib.CheckCollisionPointRec(mousePos, layoutRect) && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                    _selectedLayout = layout;
                    _settingsManager.UpdateSettings(new MazeSettings(
                        layout,
                        _settingsManager.Settings.Width,
                        _settingsManager.Settings.Height,
                        _settingsManager.Settings.FramesPerSecond
                    )); 
                }
            }

            return currentY + previewWidth;

        }

        private Button ExitButton() { 
            string text = "Exit";

            float targetWidth = _settingsWindow.Width * 0.1f;
            float targetHeight = _settingsWindow.Height * 0.08f;
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
                _settingsWindow.X + _settingsWindow.Width - _vSpace - width,
                _settingsWindow.Y + _hSpace,
                // _settingsWindow.Y + _hSpace + _instructionScrollY,
                width,
                height,
                text,
                scaledFontSize,
                _onExitAction
            );
        }

        public void Cleanup() {
            _previewManager?.Cleanup();
        }

    }
}