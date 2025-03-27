using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components.screens {

    public class ScreenSettings : IScreen {

        public bool IsInitialized { get; private set; } = false;
        
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
        private string? _activeInputBox = null;
        private string _currentInputText = "";
        private Dictionary<string, string> _inputErrors = new Dictionary<string, string>();

        MazeSettingsModel _settingsManager;
        
        // TODO: The fontsize on the "restart" button on the 4 mazelayout is not correct.
        
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

            int titleFontSize = DrawTitleAndAdvance(ref currentY, "Settings", _settingsWindow.Width, true);
    
            int layoutFontSize = DrawTitleAndAdvance(ref currentY, "Layout:", titleFontSize);
            currentY += DrawLayoutSection(currentY, mousePos, layoutFontSize) + _hSpace;

            int dimSectionFontSize = DrawTitleAndAdvance(ref currentY, "Dimensions:", titleFontSize);
            currentY += DrawInputBoxForDimensions(currentY, mousePos, dimSectionFontSize, "X:", settings => settings.Width) + _hSpace;
            currentY += DrawInputBoxForDimensions(currentY, mousePos, dimSectionFontSize, "Y:", settings => settings.Height) + _hSpace;


            // currentY += sectionTitleSize.Y + _hSpace;






            Button exitButton = ExitButton();
            exitButton.Update(mousePos);
            exitButton.Draw();

        }


        // Helper method for drawing a title and advancing the currentY
        // Returns the fontsize of the title used to scale other elements
        private int DrawTitleAndAdvance(ref float currentY, string title, float scaleElement, bool isWindowTitle = false) {
            var (height, fontSize) = DrawTitle(currentY, title, scaleElement, isWindowTitle);
            currentY += height + _hSpace;
            return fontSize; 
        }


        private (float height, int fontSize) DrawTitle(float currentY, string title, float scaleElement , bool isWindowTitle = false ) {
            
            int fontSize = isWindowTitle ? 
                Math.Clamp((int)(scaleElement * 0.05f), 20, 70) : 
                Math.Clamp((int)(scaleElement * 0.70), 12, 50);

            Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, fontSize, _textSpacing);

            
            float textX = isWindowTitle ?
                    _settingsWindow.X + (_settingsWindow.Width - textSize.X) / 2 :  // Center for window title
                    _settingsWindow.X + _vSpace;

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                title,
                new Vector2(textX, currentY),
                fontSize,
                _textSpacing,
                isWindowTitle ? Color.White : Color.Black
            );
            if (isWindowTitle) { // underline title
                Vector2 startPos = new Vector2(textX, currentY + textSize.Y);
                Vector2 endPos = new Vector2(textX + textSize.X, currentY + textSize.Y);
                Raylib.DrawLineEx(startPos, endPos, 2, Color.Black);
            }

            return (textSize.Y, fontSize);
        }

        private float DrawInputBoxForDimensions(float currentY, Vector2 mousePos, int scaleElement, string title, Func<MazeSettings, int> getDimensionValue) {
            int fontSize = Math.Clamp((int)(scaleElement * 0.70f), 12, 50); 
            Vector2 titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, fontSize, _textSpacing);

            Vector2 inputWidth4Chars = Raylib.MeasureTextEx(Raylib.GetFontDefault(), "9999", fontSize, _textSpacing);

            float inputBoxWidth = inputWidth4Chars.X * 1.1f; 
            float inputBoxHeight = titleSize.Y * 1.5f; 

            float labelY = currentY + (inputBoxHeight - titleSize.Y) / 2;
            float labelX = _settingsWindow.X + _vSpace;
            
            // Draw label
            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                title,
                new Vector2(labelX, labelY),
                fontSize,
                _textSpacing,
                Color.Black
            ); 

            Rectangle inputBox = new Rectangle(
                labelX + titleSize.X + _vSpace,
                currentY,
                inputBoxWidth,
                inputBoxHeight
            );
            
            bool isActive = _activeInputBox == title;

            // Draw input box
            // TODO: THINK MORE ABOUT THE COLOR SCHEME
            Raylib.DrawRectangleRec(inputBox, isActive ? Color.White : Color.White);
            Raylib.DrawRectangleLinesEx(inputBox, 1, isActive ? Color.Green : Color.Black);

            string displayText; 
            if (isActive) {
                displayText = _currentInputText;

                // Keyboard input
                int key = Raylib.GetCharPressed();
                while (key > 0) {
                    if (char.IsDigit((char)key) && _currentInputText.Length < 4 ) {
                        _currentInputText += (char)key; 
                    }
                    key = Raylib.GetCharPressed();
                }
                // backspace
                if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && _currentInputText.Length > 0) {
                    _currentInputText = _currentInputText.Substring(0, _currentInputText.Length - 1);
                }

                // Enter key to submit 
                if (Raylib.IsKeyPressed(KeyboardKey.Enter)) {
                    if (int.TryParse(_currentInputText, out int newValue)) {
                        if (newValue < MazeSettings.MIN_DIMENSION || newValue > MazeSettings.MAX_DIMENSION) {
                            _inputErrors[title] = $"Value must be between {MazeSettings.MIN_DIMENSION} and {MazeSettings.MAX_DIMENSION}";
                        } else {
                            _inputErrors.Remove(title);

                            if (title == "X:") {
                                _settingsManager.UpdateSize(newValue, _settingsManager.Settings.Height);
                            } else if (title == "Y:") {
                                _settingsManager.UpdateSize(_settingsManager.Settings.Width, newValue);
                            }
                        }
                    } else {
                        _inputErrors[title] = "Invalid input";
                    }
                    _activeInputBox = null; // deactivate input box
                    Raylib.SetMouseCursor(MouseCursor.Default);
                }

                // Cancel by clicking outside the input box
                if (!Raylib.CheckCollisionPointRec(mousePos, inputBox) && Raylib.IsMouseButtonPressed(MouseButton.Left) ) {
                    _activeInputBox = null; // deactivate input box
                    Raylib.SetMouseCursor(MouseCursor.Default);
                }

            } else {
                // Show the current value (not editing)
                displayText = getDimensionValue(_settingsManager.Settings).ToString();
                displayText = displayText.Length <= 4
                    ? displayText.PadLeft(4, '0')
                    : displayText.Substring(0, 4);
            }

            Vector2 settingSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), displayText, fontSize, _textSpacing);
            // Center text in input box on X and Y
            float settingTextX = inputBox.X + (inputBox.Width - settingSize.X) / 2;
            float settingTextY = inputBox.Y + (inputBox.Height - settingSize.Y) / 2;

            Raylib.DrawTextEx(
                Raylib.GetFontDefault(),
                displayText,
                new Vector2(settingTextX, settingTextY),
                fontSize,
                _textSpacing,
                Color.Black
            );

            if (isActive && ((int)(Raylib.GetTime() * 2) % 2 == 0)) {
                float cursorX = settingTextX + settingSize.X;
                Raylib.DrawLine((int)cursorX, (int)settingTextY, (int)cursorX, (int)(settingTextY + settingSize.Y), Color.Black);
            }


            if (_inputErrors.TryGetValue(title, out string? errorMsg)) {
                float errorFontSize = fontSize * 0.6f;
                Vector2 errorSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), errorMsg, errorFontSize, _textSpacing);

                float errorX = inputBox.X + inputBox.Width + _vSpace;
                float errorY = currentY + (inputBoxHeight - errorSize.Y) / 2;

                Raylib.DrawTextEx(
                    Raylib.GetFontDefault(),
                    errorMsg,
                    new Vector2(errorX, errorY),
                    errorFontSize,
                    _textSpacing,
                    Color.Red
                );
            }

            // User interaction
            if (Raylib.CheckCollisionPointRec(mousePos, inputBox) && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                Raylib.SetMouseCursor(MouseCursor.IBeam);
                _activeInputBox = title;
                _currentInputText = getDimensionValue(_settingsManager.Settings).ToString();
                
                _inputErrors.Remove(title);
            }


            return inputBoxHeight;
        }



        private float DrawLayoutSection(float currentY, Vector2 mousePos, int scaleElement) {        
            string[] layoutNames = Enum.GetNames(typeof(MazeLayout));
            string longestName = layoutNames.OrderByDescending(name => name.Length).First();

            float labelFontSize = Math.Clamp(scaleElement * 0.45f, 8, 20); // Labels are 45% of the section font size

            Vector2 longestTextSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), longestName, labelFontSize, _textSpacing);

            float previewSize = longestTextSize.X * 1.1f;
            // label height since all labels use same font size
            float labelHeight = Raylib.MeasureTextEx(Raylib.GetFontDefault(), "Sample", labelFontSize, _textSpacing).Y;


            float availableWidth = _settingsWindow.Width - (2 * _vSpace);
        
            float totalPreviewsWidth = 4 * previewSize;
            float remainingSpace = availableWidth - totalPreviewsWidth;
            float spacing = Math.Clamp(remainingSpace / 3, 10, 35); 

            
            // Center the layout options
            float totalWidth = (4 * previewSize) + (3 * spacing);
            float startX = _settingsWindow.X + _vSpace +  (_settingsWindow.Width - totalWidth) / 2;
            
            for (int i = 0; i < 4; i++) {
                MazeLayout layout = (MazeLayout)i;
                float x = startX + i * ( previewSize +  spacing);
                bool isSelected = layout == _selectedLayout;

                _previewManager!.DrawLayoutOption(layout, x, currentY, previewSize, previewSize, isSelected);

                string layoutName = layout.ToString();
                Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), layoutName, labelFontSize, _textSpacing);
                
                float textX = x + (previewSize - textSize.X) / 2;
                float textY = currentY + previewSize + _hSpace;

                Raylib.DrawTextEx(
                    Raylib.GetFontDefault(),
                    layoutName,
                    new Vector2(textX, textY),
                    labelFontSize,
                    _textSpacing,
                    Color.Black
                );

                // Handle users selection of layout
                Rectangle layoutRect = new Rectangle(x, currentY, previewSize, previewSize);                
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

            return previewSize + labelHeight;

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