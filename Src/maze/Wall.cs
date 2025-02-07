namespace MazeGen.maze.wall {
    
    // bitmask for walls
    [Flags]
    public enum Wall : int {
        None  = 0,
        North = 1 << 0,  // 1
        East  = 1 << 1,  // 2
        South = 1 << 2,  // 4
        West  = 1 << 3,   // 8
    }   
} // namespace MazeGen.maze.wall