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
            Settings = Settings with {
                Layout = newLayout
            };
        }

        // Update only the size of the settings object.
        public void UpdateSize(int newWidth, int newHeight) {
            Settings = Settings with {
                Width = newWidth,
                Height = newHeight
            };
        }

        // Update only the frames per second of the settings object.
        public void UpdateFramesPerSecond(int newFramesPerSecond) {
            Settings = Settings with {
                FramesPerSecond = newFramesPerSecond
            }; 
        }

        public void UpdateAlgorithm(AlgorithmType newAlgorithm) {
            Settings = Settings with {
                Alg = newAlgorithm
            };
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
