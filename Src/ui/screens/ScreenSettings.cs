using System.Numerics;
using Raylib_cs;

namespace MazeGen.ui.components.screens {

    public class ScreenSettings : AbstractScreen{

        private MazeLayoutPreview? _previewManager; 
        private MazeLayout _selectedLayout;
        private string? _activeInputBox = null;
        private string _currentInputText = "";
        private Dictionary<string, string> _inputErrors = new Dictionary<string, string>();

        MazeSettingsModel _settingsManager;
        
        // TODO: The fontsize on the "restart" button on the 4 mazelayout is not correct.
        
        public ScreenSettings(int parentWindowWidth, int parentWindowHeight, Action onExitAction, MazeSettingsModel settingsManager) 
            : base(parentWindowWidth, parentWindowHeight, onExitAction) {
            _settingsManager = settingsManager;
            } 
      
      
        public override void Initialize(){
            IsInitialized = true;
            _previewManager = new MazeLayoutPreview(150, 150);
            _selectedLayout = _settingsManager.Settings.Layout;    
        }

        public override void Draw(Vector2 mousePos) {
            base.Draw(mousePos);
        } 

        protected override float DrawContent(Vector2 mousePos, float currentY) {
            

            int titleFontSize = DrawTitleAndAdvance(ref currentY, "Settings", _screenWindow.Width, true);
    
            int layoutFontSize = DrawTitleAndAdvance(ref currentY, "Layout:", titleFontSize);
            currentY += DrawLayoutSection(currentY, mousePos, layoutFontSize) + _hSpace;

            int dimSectionFontSize = DrawTitleAndAdvance(ref currentY, "Dimensions:", titleFontSize);
            currentY += DrawInputBox(currentY, mousePos, dimSectionFontSize, "X:", settings => settings.Width, newValue => _settingsManager.UpdateSize(newValue, _settingsManager.Settings.Height)) + _hSpace;
            currentY += DrawInputBox(currentY, mousePos, dimSectionFontSize, "Y:", settings => settings.Height, newValue => _settingsManager.UpdateSize(newValue, _settingsManager.Settings.Width)) + _hSpace;
            int speedSectionFontSize = DrawTitleAndAdvance(ref currentY, "Animation Speed:", titleFontSize);
            currentY += DrawInputBox(currentY, mousePos, speedSectionFontSize, "S:", settings => settings.FramesPerSecond, newValue => _settingsManager.UpdateFramesPerSecond(newValue)) + _hSpace;

            currentY += _hSpace * 3; // Add some padding at the bottom
            return currentY;
        }

        private float DrawInputBox(float currentY, Vector2 mousePos, int scaleElement, string title, Func<MazeSettings, int> getValue, Action<int> updateValue) {
            int fontSize = Math.Clamp((int)(scaleElement * 0.70f), 12, 50); 
            Vector2 titleSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), title, fontSize, _textSpacing);

            Vector2 inputWidth3Chars = Raylib.MeasureTextEx(Raylib.GetFontDefault(), "999", fontSize, _textSpacing);

            float inputBoxWidth = inputWidth3Chars.X * 1.1f; 
            float inputBoxHeight = titleSize.Y * 1.5f; 

            float labelY = currentY + (inputBoxHeight - titleSize.Y) / 2;
            float labelX = _screenWindow.X + _vSpace;
            
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

            Raylib.DrawRectangleRec(inputBox, Color.White);
            Raylib.DrawRectangleLinesEx(inputBox, 1, isActive ? Color.Green : Color.Black);

            string displayText; 
            if (isActive) {
                displayText = _currentInputText;

                // Keyboard input
                int key = Raylib.GetCharPressed();
                while (key > 0) {

                    int maxChars = (title == "S:") ? 2 : 3; // speed is 2 digits, others are 3

                    if (char.IsDigit((char)key) && _currentInputText.Length < maxChars ) {
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
                        
                        int minValue, maxValue; 

                        if (title == "X:" || title == "Y:") {
                            minValue = MazeSettings.MIN_DIMENSION;
                            maxValue = MazeSettings.MAX_DIMENSION;
                        } else if (title == "S:") {
                            minValue = MazeSettings.MIN_FPS;
                            maxValue = MazeSettings.MAX_FPS;
                        } else { // default values 
                            minValue = 1;
                            maxValue = 60;
                        }

                        if (newValue < minValue || newValue > maxValue) {
                            _inputErrors[title] = $"Value must be between {minValue} and {maxValue}";
                        } else {
                            _inputErrors.Remove(title);
                            updateValue(newValue);
 
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
                displayText = getValue(_settingsManager.Settings).ToString();
                displayText = displayText.Length <= 3
                    ? displayText
                    : displayText.Substring(0, 3);
            }

            Vector2 settingSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), displayText, fontSize, _textSpacing);
            
            float settingTextX = inputBox.X + _vSpace * 0.2f;
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
                _currentInputText = getValue(_settingsManager.Settings).ToString();
                
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


            float availableWidth = _screenWindow.Width - (2 * _vSpace);
        
            float totalPreviewsWidth = 4 * previewSize;
            float remainingSpace = availableWidth - totalPreviewsWidth;
            float spacing = Math.Clamp(remainingSpace / 3, 10, 35); 

            
            // Center the layout options
            float totalWidth = (4 * previewSize) + (3 * spacing);
            float startX = _screenWindow.X + _vSpace +  (_screenWindow.Width - totalWidth) / 2;
            
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
      
        public override void Cleanup() {
            _previewManager?.Cleanup();
        }

    }
}