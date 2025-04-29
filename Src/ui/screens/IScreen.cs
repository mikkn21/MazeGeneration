using System.Numerics;

namespace MazeGen.ui.components.screens {

    public interface IScreen {

        bool IsInitialized { get; }

        void Initialize();

        void Draw(Vector2 mousePos); 


        // Not all screens need these methods
        void Cleanup() {} // Default implementation
        void DrawBackgroundMaze() {} // Default implementation 

    }


}