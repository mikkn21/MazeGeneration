namespace MazeGen.maze {


    // Bitmasking enum
    [Flags]
    public enum Wall : int {
        None  = 0,
        North = 1 << 0,  // 1
        East  = 1 << 1,  // 2
        South = 1 << 2,  // 4
        West  = 1 << 3   // 8
    }

    public class Maze {
        
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Wall[,] cells;

        public Maze(int width, int height) {

            Width = width;
            Height = height;
            cells = new Wall[width, height];

            // init maze with all walls 
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    cells[x, y] = Wall.North | Wall.East | Wall.South | Wall.West;
                }
            }
        }


        public Wall getWalls(int x, int y) {
            throw new NotImplementedException();
        }

        public void setWall(int x, int y, Wall wall) {
            throw new NotImplementedException();
        }

        public void addWall(int x, int y, Wall wall) {
            throw new NotImplementedException();
        }



        public void removeWallBetween(int x1, int y1, int x2, int y2) {
            throw new NotImplementedException();
        }

        //  Quick check for whether a given wall is present
        public bool HasWall(int x, int y, Wall wall) {
            if (!InBounds(x, y)) {
               return true; 
            } 
            return (cells[x, y] & wall) != 0;
        }


        // bounds check
        public bool InBounds(int x, int y) {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }   




    } // class Maze
} // namespace MazeGen.maze