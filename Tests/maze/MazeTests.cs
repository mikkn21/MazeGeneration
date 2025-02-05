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
            maze.MarkCell(2, 2);
            Assert.True(maze.HasVisited(2, 2));
            
            // Act & Assert second visit
            maze.MarkCell(2, 2);
            Assert.True(maze.HasVisited(2, 2));
        }

        [Fact]
        public void TestGetNeighbours() {
            // Arrange
            Maze maze = new Maze(5, 5);

            // Act
            List<(Wall wall, char dir)> neighbours = maze.GetNeighbours(2, 2);
            
            // Assert
            Assert.Equal(4, neighbours.Count);
            Assert.Contains(neighbours, n => n.dir == 'N' && (n.wall & Wall.North) == Wall.North);
            Assert.Contains(neighbours, n => n.dir == 'S' && (n.wall & Wall.South) == Wall.South);
            Assert.Contains(neighbours, n => n.dir == 'E' && (n.wall & Wall.East) == Wall.East);
            Assert.Contains(neighbours, n => n.dir == 'W' && (n.wall & Wall.West) == Wall.West);
        }

        [Fact]
        public void TestGetNeighboursAtBorder() {
            // Arrange
            Maze maze = new Maze(5, 5);

            // Act
            List<(Wall wall, char dir)> neighbours = maze.GetNeighbours(0, 0);
            
            // Assert
            Assert.Equal(2, neighbours.Count);
            Assert.Contains(neighbours, n => n.dir == 'S' && (n.wall & Wall.South) == Wall.South);
            Assert.Contains(neighbours, n => n.dir == 'E' && (n.wall & Wall.East) == Wall.East);
        }


    } // class MazeTests
} // namespace Mazegen.Tests.maze