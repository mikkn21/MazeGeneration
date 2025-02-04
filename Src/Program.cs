using MazeGen.maze;
using MazeGen.maze.draw;

class Program{
    static void Main(string[] args){
        Maze maze = new Maze(10, 10);

        int cellSize = 50;
        int wallThickness = 3;

        MazeDraw mazeDraw = new MazeDraw(maze, cellSize, wallThickness);
        mazeDraw.Draw();
    }



} // class Program