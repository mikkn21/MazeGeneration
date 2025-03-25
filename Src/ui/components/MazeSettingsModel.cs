using MazeGen.ui.components;
using MazeGen.ui.components.screens;


// Observer Pattern for MazeSettings
// This class is the model for the MazeSettings object.
public class MazeSettingsModel {
        private MazeSettings _settings;
        

        public MazeSettings Settings {
            get => _settings;
            private set {
                _settings = value;
                OnSettingsChanged(new MazeSettingsChangedEventArgs(_settings));
            }
        }

        public event EventHandler<MazeSettingsChangedEventArgs>? SettingsChanged;

        public MazeSettingsModel(MazeSettings initialSettings) {
            _settings = initialSettings;
        }
        
        // Update the entire settings object.
        public void UpdateSettings(MazeSettings newSettings) {
            Settings = newSettings;
        }

        // Update only the layout of the settings object.
        public void UpdateLayout(MazeLayout newLayout) {
            Settings = new MazeSettings(newLayout, _settings.Width, _settings.Height, _settings.FramesPerSecond);
        }

        // Update only the size of the settings object.
        public void UpdateSize(int newWidth, int newHeight) {
            Settings = new MazeSettings(_settings.Layout, newWidth, newHeight, _settings.FramesPerSecond);
        }

        // Update only the frames per second of the settings object.
        public void UpdateFramesPerSecond(int newFramesPerSecond) {
            Settings = new MazeSettings(_settings.Layout, _settings.Width, _settings.Height, newFramesPerSecond);
        }


        protected virtual void OnSettingsChanged(MazeSettingsChangedEventArgs e) {
            SettingsChanged?.Invoke(this, e);
        }
    }

    public class MazeSettingsChangedEventArgs : EventArgs {
        public MazeSettings NewSettings { get; }
        public MazeSettingsChangedEventArgs(MazeSettings newSettings) {
            NewSettings = newSettings;
        }
    }
