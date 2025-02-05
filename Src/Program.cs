using MazeGen.Algorithms;
using MazeGen.maze;
using MazeGen.maze.draw;

class Program{
    static void Main(string[] args){
        Maze maze = new Maze(3,3);

        int cellSize = 200;
        int wallThickness = 2;

        Backtracking backtracking = new Backtracking();
        maze = backtracking.GenerateMaze(maze);

        MazeDraw mazeDraw = new MazeDraw(maze, cellSize, wallThickness);
        mazeDraw.Draw();
    }



} // class Program