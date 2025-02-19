using MazeGen.Algorithms;
using MazeGen.maze;
using MazeGen.ui;
using MazeGen.ui.app;

class Program{
    static void Main(string[] args){
        Maze maze1 = new (10,10);
        Maze maze2 = maze1.Copy();

        int cellSize = 50;
        int wallThickness = 5;
        int framesPerStep = 1;

        Backtracking generator1 = new Backtracking(maze1);
        Backtracking generator2 = new Backtracking(maze2);

        MazeWindow mazeWindow1 = new (maze1, cellSize, generator1, wallThickness, framesPerStep);
        MazeWindow mazeWindow2 = new (maze2, cellSize, generator2, wallThickness, framesPerStep);

        MazeWindow[] mazeWindows = [mazeWindow1, mazeWindow2];

        MazeGenApp visualizer = new (mazeWindows);
        visualizer.Run();
    }



} // class Program