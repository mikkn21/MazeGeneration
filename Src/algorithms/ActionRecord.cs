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
        
        // For backtracking moves:
        public Tile? PoppedTile { get; }

        // Constructor for a forward move:
        public ActionRecord(Tile fromTile, Tile toTile) {
            ActionType = MazeActionType.Forward;
            FromTile = fromTile;
            ToTile = toTile;
        }
    
        // Constructor for a backtracking move:
        public ActionRecord(Tile poppedTile) {
            ActionType = MazeActionType.Backtrack;
            PoppedTile = poppedTile;
        }

        public override string ToString() {
            if (ActionType == MazeActionType.Forward) {
                return $"ActionRecord: Forward from {FromTile} to {ToTile}";
            }
            else {
                return $"ActionRecord: Backtrack from {PoppedTile}";
            }
        }
    }
} 