using MazeGen.maze.tile;


namespace MazeGen.Algorithms.actionrecord {
    public class ActionRecord {
        public Tile Tile { get; set; }

        public ActionRecord(Tile tile) {
            Tile = tile;
        }
    }
} 