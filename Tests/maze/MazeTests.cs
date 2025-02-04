using MazeGen.maze;
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
        public void TestRemoveWall(){
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act
            maze.RemoveWallBetween(4,4,4,5);
             
            //Assert
            Assert.False(maze.HasWall(4,4, Wall.South));
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
            maze.RemoveWallBetween(4,4,4,5);
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
        public void TestVisitCellAndHasVisited() {
            var maze = new Maze(5, 5);

            // Assert initial state
            Assert.False(maze.HasVisited(2,2));

            // Act & Assert first visit
            Assert.True(maze.VisitCell(2, 2));
            Assert.True(maze.HasVisited(2, 2));
            
            // Act & Assert second visit
            Assert.False(maze.VisitCell(2, 2));
            Assert.True(maze.HasVisited(2, 2));
        }



    } // class MazeTests
} // namespace Mazegen.Tests.maze