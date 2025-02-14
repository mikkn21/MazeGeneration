using MazeGen.maze.tile;
using MazeGen.maze.wall;


namespace MazeGen.Algorithms.actionrecord {
    public enum MazeActionType {
        Forward,
        Backtrack
    }

    public class ActionRecord {

        public MazeActionType ActionType { get; }
        
        // For forward moves:
        public Tile? FromTile { get; }
        public Tile? ToTile { get; }
        public Wall? RemovedWallFrom { get; }
        public Wall? RemovedWallTo { get; }
        
        // For backtracking moves:
        public Tile? PoppedTile { get; }

        // Constructor for a forward move:
        public ActionRecord(Tile fromTile, Tile toTile, Wall removedWallFrom, Wall removedWallTo) {
            ActionType = MazeActionType.Forward;
            FromTile = fromTile;
            ToTile = toTile;
            RemovedWallFrom = removedWallFrom;
            RemovedWallTo = removedWallTo;
        }
    
        // Constructor for a backtracking move:
        public ActionRecord(Tile poppedTile) {
            ActionType = MazeActionType.Backtrack;
            PoppedTile = poppedTile;
        }

    }

} 