using MazeGen.ui.components.screens;

namespace MazeGen.ui.components {
    public class MazeSettings {

        public const int MIN_DIMENSION = 5;
        public const int MAX_DIMENSION = 100;
        public const int MIN_FPS = 1;
        public const int MAX_FPS = 60;

        public MazeLayout Layout { get; }
        public int Width { get; }
        public int Height { get; }
        public int FramesPerSecond { get; }
 

        public MazeSettings(MazeLayout layout = MazeLayout.TwoMazes, int width = 10, int height = 10, int framesPerSecond = 1) {
            Layout = layout;
            Width = Math.Clamp(width, MIN_DIMENSION, MAX_DIMENSION);
            Height = Math.Clamp(height, MIN_DIMENSION, MAX_DIMENSION);
            FramesPerSecond = Math.Clamp(framesPerSecond, MIN_FPS, MAX_FPS);

        }
    }
}