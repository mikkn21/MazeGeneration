using MazeGen.ui.components.screens;

namespace MazeGen.ui.components {
    public class MazeSettings {
        public MazeLayout Layout { get; }
        public int Width { get; }
        public int Height { get; }
        public int FramesPerSecond { get; }
 

        public MazeSettings(MazeLayout layout = MazeLayout.TwoMazes, int width = 10, int height = 10, int framesPerSecond = 1) {
            Layout = layout;
            Width = width;
            Height = height;
            FramesPerSecond = framesPerSecond;
            
        }
    }
}