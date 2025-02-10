using MazeGen.Algorithms;
using MazeGen.maze;
using MazeGen.maze.draw;
using MazeGen.maze.step;

class Program{
    static void Main(string[] args){
        Maze maze = new Maze(5,5);

        int cellSize = 100;
        int wallThickness = 2;

        Backtracking generator = new Backtracking(maze);

        MazeDraw drawer = new MazeDraw(maze, cellSize, wallThickness);

        drawer.Draw(generator); 

    }



} // class Program