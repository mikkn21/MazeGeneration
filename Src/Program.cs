using MazeGen.Algorithms;
using MazeGen.maze;
using MazeGen.maze.draw;
using MazeGen.maze.step;

class Program{
    static void Main(string[] args){
        Maze maze = new Maze(5,5);

        int cellSize = 100;
        int wallThickness = 2;

        Backtracking backtracking = new Backtracking();
        List<MazeStep> mazeSteps = backtracking.GenerateMaze(maze);

        MazeDraw mazeDraw = new MazeDraw(maze, mazeSteps, cellSize, wallThickness);
        mazeDraw.Draw();
    }



} // class Program