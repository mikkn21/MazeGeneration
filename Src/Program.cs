using MazeGen.Algorithms;
using MazeGen.maze;
using MazeGen.maze.draw;

class Program{
    static void Main(string[] args){
        Maze maze = new Maze(10,10);

        int cellSize = 50;
        int wallThickness = 2;
        int framesPerStep = 1;

        Backtracking generator = new Backtracking(maze);

        MazeDraw drawer = new MazeDraw(maze, cellSize, wallThickness, framesPerStep);

        drawer.Draw(generator); 

    }



} // class Program