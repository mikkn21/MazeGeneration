using MazeGen.maze;
using MazeGen.maze.tile;
using MazeGen.maze.wall;
using Xunit;
using Xunit.Sdk;


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
                    Assert.True(maze.HasWall(x, y, Wall.North));
                    Assert.True(maze.HasWall(x, y, Wall.East));
                    Assert.True(maze.HasWall(x, y, Wall.South));
                    Assert.True(maze.HasWall(x, y, Wall.West));
                }
            }
        }

        [Fact]
        public void TestWallPresent() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act and Assert
            Assert.True(maze.HasWall(1,1, Wall.North));
        }

        [Fact]
        public void TestWallOutsideMaze() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act and Assert
            Assert.True(maze.HasWall(6,1, Wall.None)); // If the wall is outside the maze then it exists
        }

        [Fact]
        public void TestRemoveWallNorth(){
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            maze.RemoveWallBetween(1, 1, 1, 0);
             
            //Assert
            Assert.False(maze.HasWall(1,1, Wall.North));
            Assert.False(maze.HasWall(1,0, Wall.South));
        }

        [Fact]
        public void TestRemoveWallSouth(){
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            maze.RemoveWallBetween(1, 1, 1, 2);
             
            //Assert
            Assert.False(maze.HasWall(1,1, Wall.South));
            Assert.False(maze.HasWall(1,2, Wall.North));
        }

        [Fact]
        public void TestRemoveWallEast(){
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            maze.RemoveWallBetween(1, 1, 2, 1);
             
            //Assert
            Assert.False(maze.HasWall(1,1, Wall.East));
            Assert.False(maze.HasWall(2,1, Wall.West));
        }

        [Fact] 
        public void TestRemoveWallWest(){
            // Arrange
            Maze maze = new Maze(3, 3);
            
            // Act
            maze.RemoveWallBetween(1, 1, 0, 1);
             
            //Assert
            Assert.False(maze.HasWall(1,1, Wall.West));
            Assert.False(maze.HasWall(0,1, Wall.East));
        }

        [Fact]
        public void TestRemoveWallError() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act and Assert
            Assert.Throws<ArgumentException>(() => maze.RemoveWallBetween(10,10,4,5));
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
                    Assert.True(mazeCopy.HasWall(x, y, Wall.North));
                    Assert.True(mazeCopy.HasWall(x, y, Wall.East));
                    Assert.True(mazeCopy.HasWall(x, y, Wall.South));
                    Assert.True(mazeCopy.HasWall(x, y, Wall.West));
                }
            }
        }

        [Fact]
        public void TestCopyAfterModification() {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act
            maze.RemoveWallBetween(3, 3, 3, 4);
            var mazeCopy = maze.Copy();
            
            // Assert
            Assert.Equal(maze.Width, mazeCopy.Width);
            Assert.Equal(maze.Height, mazeCopy.Height);
            Assert.Equal(maze.HasWall(3, 3, Wall.South), mazeCopy.HasWall(3, 3, Wall.South));
            Assert.Equal(maze.HasWall(3, 4, Wall.North), mazeCopy.HasWall(3, 4, Wall.North));
        }

        [Fact]
        public void TestMarkCellAndHasVisited() {
            var maze = new Maze(5, 5);

            // Assert initial state
            Assert.False(maze.HasVisited(2,2));

            // Act & Assert first visit
            maze.MarkTile(2, 2);
            Assert.True(maze.HasVisited(2, 2));
            
            // Act & Assert second visit
            maze.MarkTile(2, 2);
            Assert.True(maze.HasVisited(2, 2));
        }

        [Fact]
        public void TestGetNeighbours() {
            // Arrange
            Maze maze = new Maze(5, 5);

            // Act
            List<(Tile tiles, char dir)> neighbours = maze.GetNeighbours(2, 2);
            
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
            List<(Tile tiles, char dir)> neighbours = maze.GetNeighbours(0, 0);
            
            // Assert
            Assert.Equal(2, neighbours.Count);
            Assert.Contains(neighbours, n => n.dir == 'S' && (n.tiles.Walls & Wall.South) == Wall.South);
            Assert.Contains(neighbours, n => n.dir == 'E' && (n.tiles.Walls & Wall.East) == Wall.East);
        }

        [Fact]
        public void TestCloneWithModifications()
        {
            // Arrange
            var original = new Maze(3, 3);
            original.MarkTile(1, 1);
            original.RemoveWallBetween(1, 1, 1, 0);

            // Act
            var clone = original.Copy();
            clone.MarkTile(0, 0);  // Modify clone
            clone.RemoveWallBetween(0, 0, 1, 0);  // Modify clone's walls

            // Assert
            // Verify original state remains unchanged
            Assert.True(original.HasVisited(1, 1));
            Assert.False(original.HasVisited(0, 0));
            Assert.False(original.HasWall(1, 1, Wall.North));
            Assert.True(original.HasWall(0, 0, Wall.East));

            // Verify clone has both original and new modifications
            Assert.True(clone.HasVisited(1, 1));
            Assert.True(clone.HasVisited(0, 0));
            Assert.False(clone.HasWall(1, 1, Wall.North));
            Assert.False(clone.HasWall(0, 0, Wall.East));
        }


    } // class MazeTests
} // namespace Mazegen.Tests.maze