using MazeGen.ui.components.screens;

namespace MazeGen.ui.components {

    public enum AlgorithmType {
        Backtracking, 
        // prim,
        // Kruskal,
        // ...
    }

    public record MazeSettings {

        public const int MIN_DIMENSION = 5;
        public const int MAX_DIMENSION = 100;
        public const int MIN_FPS = 1;
        public const int MAX_FPS = 60;

        public MazeLayout Layout { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public int FramesPerSecond { get; init; }

        public AlgorithmType Alg { get; init; }

 

        public MazeSettings(MazeLayout layout = MazeLayout.TwoMazes, int width = 10, int height = 10, int framesPerSecond = 1, AlgorithmType alg = AlgorithmType.Backtracking) {
            Layout = layout;
            Width = Math.Clamp(width, MIN_DIMENSION, MAX_DIMENSION);
            Height = Math.Clamp(height, MIN_DIMENSION, MAX_DIMENSION);
            FramesPerSecond = Math.Clamp(framesPerSecond, MIN_FPS, MAX_FPS);
            Alg = alg;
        }
    }
}