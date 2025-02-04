namespace MazeGen.maze {


    // Bitmasking enum
    [Flags]
    public enum Wall : int {
        None  = 0,
        North = 1 << 0,  // 1
        East  = 1 << 1,  // 2
        South = 1 << 2,  // 4
        West  = 1 << 3,   // 8
        visited = 1 << 4 // 16
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

        // Visists a cell 
        // Returns true if the cell has not been visited before
        // Returns false if the cell has already been visited before 
        public Boolean VisitCell(int x, int y) {
            if (HasVisited(x,y)) {
                return false;
            }
            cells[x,y] |= Wall.visited;
            return true;
        }

        public void SetWall(int x, int y, Wall wall) {
            throw new NotImplementedException();
        }

        public void AddWall(int x, int y, Wall wall) {
            throw new NotImplementedException();
        }

        public Boolean HasVisited(int x, int y) {
            Wall cell = cells[x,y]; 
            return (cell & Wall.visited) == Wall.visited;
        }


        public void RemoveWallBetween(int x1, int y1, int x2, int y2) {
            if (!(InBounds(x1, y1) && InBounds(x2, y2))) {
                throw new ArgumentException("Invalid coordinates");
            }

            Wall cell1 = cells[x1, y1];
            Wall cell2 = cells[x2, y2];
            
            // Check if cell2 is north of cell1
            if (x1 == x2  && y1 == y2 + 1) {
                cell1 &= ~Wall.North;
                cell2 &= ~Wall.South;
            }
            // check if cell2 is east of cell1 
            else if (x1 == x2 + 1 && y1 == y2) {
                cell1 &= ~Wall.East;
                cell2 &= ~Wall.West;
            }
            // check if cell2 is south of cell1 
            else if (x1 == x2 && y1 == y2 - 1) {
                cell1 &= ~Wall.South;
                cell2 &= ~Wall.North;
            }
            // check if cell2 is west of cell1
            else if (x1 == x2 - 1 && y1 == y2) {
                cell1 &= ~Wall.West;
                cell2 &= ~Wall.East;
            }

            cells[x1, y1] = cell1;
            cells[x2, y2] = cell2;

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

        public Maze Copy(){
        Maze clone = new Maze(Width, Height);
        for (int x = 0; x < Width; x++){
            for (int y = 0; y < Height; y++){
                clone.cells[x, y] = this.cells[x, y];
            }
        }
        return clone;
    } 




    } // class Maze
} // namespace MazeGen.maze