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

        // Mark a cell as visisted 
        // Returns true if the cell has not been visited before
        // Returns false if the cell has already been visited before 
        public void MarkCell(int x, int y) {
            if (!HasVisited(x,y)) {
                cells[x, y] |= Wall.visited;
            }
        }

        public Boolean HasVisited(int x, int y) {
            return (cells[x,y] & Wall.visited) == Wall.visited;
        }

        public void SetWall(int x, int y, Wall wall) {
            throw new NotImplementedException();
        }

        public void AddWall(int x, int y, Wall wall) {
            throw new NotImplementedException();
        }


        public List<(Wall, char)> GetNeighbours(int x, int y) {
            List<(Wall, char)> neighbours = [];
            // North
            if (InBounds(x, y-1)) {
                neighbours.Add((cells[x, y-1], 'N'));
            }
            // South
            if (InBounds(x, y+1)) { 
                neighbours.Add((cells[x, y+1], 'S'));
            }
            // East
            if (InBounds(x+1, y)) {
                neighbours.Add((cells[x+1, y], 'E'));
            }
            // West 
            if (InBounds(x-1, y)) {
                neighbours.Add((cells[x-1, y], 'W'));   
            }
            return neighbours;
        }

        public void RemoveWallBetween(int x1, int y1, int x2, int y2) {
            if (!(InBounds(x1, y1) && InBounds(x2, y2))) {
                throw new ArgumentException("Invalid coordinates");
            }

            // Check if (x2, y2) is North of (x1, y1)
            if (x1 == x2 && y2 == y1 - 1) { 
                cells[x1, y1] &= ~Wall.North;
                cells[x2, y2] &= ~Wall.South;
            }
            // Check if (x2, y2) is South of (x1, y1)
            else if (x1 == x2 && y2 == y1 + 1) {
                cells[x1, y1] &= ~Wall.South;
                cells[x2, y2] &= ~Wall.North;
            }
            // Check if (x2, y2) is East of (x1, y1)
            else if (y1 == y2 && x2 == x1 + 1) {
                Console.WriteLine("##### Removing East wall");
                cells[x1, y1] &= ~Wall.East;
                cells[x2, y2] &= ~Wall.West;
            }
            // Check if (x2, y2) is West of (x1, y1)
            else if (y1 == y2 && x2 == x1 - 1) {
                cells[x1, y1] &= ~Wall.West;
                cells[x2, y2] &= ~Wall.East;
            } else {
                throw new ArgumentException("Cells are not adjacent");
            }

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