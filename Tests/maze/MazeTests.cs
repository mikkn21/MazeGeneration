using MazeGen.maze;
using MazeGen.maze.tile;
using MazeGen.maze.wall;

namespace Mazegen.Tests.maze
{
    public class MazeTests {

        [Fact] 
        public void MazeConstructorTest() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act and Assert
            Assert.Equal(5, maze.Width);
            Assert.Equal(5, maze.Height);
            for (int x = 0; x < 5; x++) {
                for (int y = 0; y < 5; y++) {
                    Tile t = maze.GetTile(x, y);
                    Assert.True(maze.HasWall(t, Wall.North));
                    Assert.True(maze.HasWall(t, Wall.East));
                    Assert.True(maze.HasWall(t, Wall.South));
                    Assert.True(maze.HasWall(t, Wall.West));
                }
            }
        }

        [Fact]
        public void TestWallPresent() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act and Assert
            Tile t = maze.GetTile(1, 1);
            Assert.True(maze.HasWall(t, Wall.North));
        }

        [Fact]
        public void TestGetTileOutsideMaze() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act and Assert
            Assert.Throws<ArgumentException>(() => maze.GetTile(6, 1));
        }


        [Fact]
        public void TestRemoveWallNorth(){
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            Tile t1 = maze.GetTile(1, 1);
            Tile t2 = maze.GetTile(1, 0);
            maze.RemoveWallBetween(t1, t2);
             
            //Assert
            Assert.False(maze.HasWall(t1, Wall.North));
            Assert.False(maze.HasWall(t2, Wall.South));
        }

        [Fact]
        public void TestRemoveWallSouth(){
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            Tile t1 = maze.GetTile(1, 1);
            Tile t2 = maze.GetTile(1, 2);
            maze.RemoveWallBetween(t1, t2);
             
            //Assert
            Assert.False(maze.HasWall(t1, Wall.South));
            Assert.False(maze.HasWall(t2, Wall.North));
        }

        [Fact]
        public void TestRemoveWallEast(){
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            Tile t1 = maze.GetTile(1, 1);
            Tile t2 = maze.GetTile(2, 1);
            maze.RemoveWallBetween(t1, t2);
             
            //Assert
            Assert.False(maze.HasWall(t1, Wall.East));
            Assert.False(maze.HasWall(t2, Wall.West));
        }

        [Fact] 
        public void TestRemoveWallWest(){
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            Tile t1 = maze.GetTile(1, 1);
            Tile t2 = maze.GetTile(0, 1);
            maze.RemoveWallBetween(t1, t2);
             
            //Assert
            Assert.False(maze.HasWall(t1, Wall.West));
            Assert.False(maze.HasWall(t2, Wall.East));
        }

        [Fact]
        public void TestCopy() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act
            var mazeCopy = maze.Copy();
            
            // Assert
            Assert.Equal(maze.Width, mazeCopy.Width);
            Assert.Equal(maze.Height, mazeCopy.Height);
            for (int x = 0; x < 5; x++) {
                for (int y = 0; y < 5; y++) {
                    Tile t = maze.GetTile(x, y);
                    Assert.True(mazeCopy.HasWall(t, Wall.North));
                    Assert.True(mazeCopy.HasWall(t, Wall.East));
                    Assert.True(mazeCopy.HasWall(t, Wall.South));
                    Assert.True(mazeCopy.HasWall(t, Wall.West));
                }
            }
        }

        [Fact]
        public void TestCopyAfterModification() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act
            Tile t1 = maze.GetTile(3, 3);
            Tile t2 = maze.GetTile(3, 4);
            maze.RemoveWallBetween(t1, t2);
            var mazeCopy = maze.Copy();
            
            // Assert
            Assert.Equal(maze.Width, mazeCopy.Width);
            Assert.Equal(maze.Height, mazeCopy.Height);
            Assert.Equal(maze.HasWall(t1, Wall.South), mazeCopy.HasWall(t1, Wall.South));
            Assert.Equal(maze.HasWall(t2, Wall.North), mazeCopy.HasWall(t2, Wall.North));
        }

        [Fact]
        public void TestMarkTile() {

            // Arrange
            Maze maze = new Maze(5, 5);
            Tile t = maze.GetTile(2, 2);

            // Act & Assert initial state
            Assert.True(t.State.Equals(TileState.Unvisited));
            Assert.True(t.Color.Equals(Raylib_cs.Color.Gray));

            // Act & Assert first visit
            maze.MarkTile(t);
            Assert.True(t.State.Equals(TileState.Visited));
            Assert.True(t.Color.Equals(Raylib_cs.Color.LightGray));
            
            // Act & Assert second visit
            maze.MarkTile(t);
            Assert.True(t.Color.Equals(Raylib_cs.Color.White));
            Assert.True(t.State.Equals(TileState.Selected));

            // Act & Assert third visit 
            maze.MarkTile(t);
            Assert.True(t.Color.Equals(Raylib_cs.Color.White));
            Assert.True(t.State.Equals(TileState.Selected));
        }

        [Fact]
        public void TestGetNeighbours() {
            // Arrange
            Maze maze = new Maze(5, 5);

            // Act
            Tile t = maze.GetTile(2, 2);
            List<(Tile tiles, char dir)> neighbours = maze.GetNeighbours(t);
            
            // Assert
            Assert.Equal(4, neighbours.Count);
            Assert.Contains(neighbours, n => n.dir == 'N' && (n.tiles.Walls & Wall.North) == Wall.North);
            Assert.Contains(neighbours, n => n.dir == 'S' && (n.tiles.Walls & Wall.South) == Wall.South);
            Assert.Contains(neighbours, n => n.dir == 'E' && (n.tiles.Walls & Wall.East) == Wall.East);
            Assert.Contains(neighbours, n => n.dir == 'W' && (n.tiles.Walls & Wall.West) == Wall.West);
        }

        [Fact]
        public void TestGetNeighboursAtBorder() {
            // Arrange
            Maze maze = new Maze(5, 5);

            // Act
            Tile t = maze.GetTile(0, 0);
            List<(Tile tiles, char dir)> neighbours = maze.GetNeighbours(t);
            
            // Assert
            Assert.Equal(2, neighbours.Count);
            Assert.Contains(neighbours, n => n.dir == 'S' && (n.tiles.Walls & Wall.South) == Wall.South);
            Assert.Contains(neighbours, n => n.dir == 'E' && (n.tiles.Walls & Wall.East) == Wall.East);
        }

        [Fact]
        public void TestCloneWithModifications() {
            // Arrange
            Maze original = new Maze(3, 3);
            Tile ogT1 = original.GetTile(1, 1);
            Tile ogT2 = original.GetTile(1, 0);
            Tile ogT3 = original.GetTile(0, 0);
            Tile ogT4 = original.GetTile(0, 1);
            original.MarkTile(ogT1);
            original.RemoveWallBetween(ogT1, ogT2);

            // Act
            Maze clone = original.Copy();
            Tile cloneT1 = clone.GetTile(1, 1);
            Tile cloneT2 = clone.GetTile(1, 0);
            Tile cloneT3 = clone.GetTile(0, 0);
            Tile cloneT4 = clone.GetTile(0, 1);
            clone.MarkTile(cloneT3);  
            clone.RemoveWallBetween(cloneT3, cloneT4); 

            // Assert
            Assert.NotSame(original.GetTile(1, 1), clone.GetTile(1, 1));

            // Verify original state remains unchanged
            Assert.True(ogT1.State.Equals(TileState.Visited));
            Assert.False(ogT3.State.Equals(TileState.Visited));
            Assert.False(original.HasWall(ogT1, Wall.North));
            Assert.True(original.HasWall(ogT3, Wall.South));

            // Verify clone has both original and new modifications
            Assert.True(cloneT1.State.Equals(TileState.Visited));
            Assert.True(cloneT3.State.Equals(TileState.Visited));
            Assert.False(clone.HasWall(cloneT1, Wall.North));
            Assert.False(clone.HasWall(cloneT3, Wall.South));
        }

        [Fact] 
        public void TestSetWallBetween() {
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            Tile t1 = maze.GetTile(1, 1);
            Tile t2 = maze.GetTile(1, 0);
            maze.RemoveWallBetween(t1, t2);
         
            // Assert
            Assert.False(maze.HasWall(t1, Wall.North));
            Assert.False(maze.HasWall(t2, Wall.South));

            // Act
            maze.SetWallBetween(t1, t2);

            // Assert 
            Assert.True(maze.HasWall(t1, Wall.North));
            Assert.True(maze.HasWall(t2, Wall.South));
        }

        [Fact]
        public void TestResetMaze() {
            // Arrange
            Maze maze = new Maze(3, 3);
            Tile t1 = maze.GetTile(1, 1);
            Tile t2 = maze.GetTile(1, 0);
            maze.MarkTile(t1);
            maze.RemoveWallBetween(t1, t2);

            // Act
            maze.ResetMaze();

            // Assert
            Assert.True(maze.HasWall(t1, Wall.North));
            Assert.True(maze.HasWall(t2, Wall.South));
            Assert.False(t1.State.Equals(TileState.Visited));
        }

        [Fact]
        public void TestUnmarkTile() {
            // Arrange
            Maze maze = new Maze(3, 3);
            Tile t = maze.GetTile(2, 2);
            maze.MarkTile(t);
            maze.MarkTile(t);

            // Assert initial state
            Assert.True(t.State.Equals(TileState.Selected));
            Assert.True(t.Color.Equals(Raylib_cs.Color.White));            

            // Act & Assert first unvisit
            maze.UnmarkTile(t);
            Assert.True(t.State.Equals(TileState.Visited));
            Assert.True(t.Color.Equals(Raylib_cs.Color.LightGray));
            
            // Act & Assert second visit
            maze.UnmarkTile(t);
            Assert.True(t.Color.Equals(Raylib_cs.Color.Gray));
            Assert.True(t.State.Equals(TileState.Unvisited));

            // Act & Assert third visit 
            maze.UnmarkTile(t);
            Assert.True(t.Color.Equals(Raylib_cs.Color.Gray));
            Assert.True(t.State.Equals(TileState.Unvisited));
        }


    } // class MazeTests
} // namespace Mazegen.Tests.maze