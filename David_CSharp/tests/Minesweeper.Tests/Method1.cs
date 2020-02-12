using Xunit;

namespace Minesweeper.Tests
{

    public class Method1
    {

        [Fact]
        public void SolveFirstCell()
        {
            var game = CreateGame();
            var actualSolution = game.Solve();

            Assert.Equal("No mines left to place.", actualSolution.Description);
            Assert.Equal(0, actualSolution.CellOfInterest.Column);
            Assert.Equal(0, actualSolution.CellOfInterest.Row);

            Assert.Equal(3, actualSolution.SolvedCells.Length);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 0 && c.Row == 1);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 1 && c.Row == 0);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 1 && c.Row == 1);

            var expectedBoard = "0x  1 11," +
                                "xx 3    ," +
                                "13 5 4  ," +
                                " 2   31 ," +
                                "  332  0," +
                                "     1  ," +
                                " 1 0 1  ," +
                                "        ";
            AssertBoard(expectedBoard, game);
        }


        [Fact]
        public void SolveSecondCell()
        {
            var game = CreateGame();

            game.Solve();
            var actualSolution = game.Solve();

            Assert.Equal("No mines left to place.", actualSolution.Description);
            Assert.Equal(3, actualSolution.CellOfInterest.Column);
            Assert.Equal(6, actualSolution.CellOfInterest.Row);

            Assert.Equal(8, actualSolution.SolvedCells.Length);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 2 && c.Row == 5);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 3 && c.Row == 5);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 4 && c.Row == 5);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 2 && c.Row == 6);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 4 && c.Row == 6);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 2 && c.Row == 7);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 3 && c.Row == 7);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 4 && c.Row == 7);

            var expectedBoard = "0x  1 11," +
                                "xx 3    ," +
                                "13 5 4  ," +
                                " 2   31 ," +
                                "  332  0," +
                                "  xxx1  ," +
                                " 1x0x1  ," +
                                "  xxx   ";
            AssertBoard(expectedBoard, game);
        }


        private void AssertBoard(string expectedBoard, Game actualGame)
        {
            var lines = expectedBoard.Split(',');
            var numberOfColumns = lines[0].Length;
            var numberOfRows = lines.Length;


            for (int column = 0; column < numberOfColumns; column++)
            {
                for (int row = 0; row < numberOfRows; row++)
                {
                    var expectedCell = lines[row][column].ToString();
                    var actualCell = actualGame.CellContents(column, row);
                    if (actualCell == "") actualCell = " ";

                    Assert.Equal(expectedCell, actualCell);
                }
            }
        }

        private Game CreateGame()
        {
            var details = "0   1 11," +
                          "   3    ," +
                          "13 5 4  ," +
                          " 2   31 ," +
                          "  332  0," +
                          "     1  ," +
                          " 1 0 1  ," +
                          "        ";

            var lines = details.Split(',');
            var numberOfColumns = lines[0].Length;
            var numberOfRows = lines.Length;

            var initialState = new char[numberOfColumns, numberOfRows];

            for (int column = 0; column < numberOfColumns; column++)
            {
                for (int row = 0; row < numberOfRows; row++)
                {
                    initialState[column, row] = lines[row][column];
                }
            }

            return new Game(initialState);

        }
    }
}
