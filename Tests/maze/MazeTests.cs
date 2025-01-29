using MazeGen.maze;
using Xunit;


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
        public void TestWallPresent()
        {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act and Assert
            Assert.True(maze.HasWall(1,1, Wall.North));
        }

        [Fact]
        public void TestWallOutsideMaze()
        {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act and Assert
            Assert.True(maze.HasWall(6,1, Wall.None)); // If the wall is outside the maze then it exists
        }

        [Fact]
        public void TestWallNotPresent()
        {
            // Arrange
            var maze = new Maze(5, 5);
            
            // Act
            // TODO Remove wall at 4,4 
            
            //Assert
            Assert.False(maze.HasWall(4,4, Wall.North));
        }


    } // class MazeTests
} // namespace Mazegen.Tests.maze