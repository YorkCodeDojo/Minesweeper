using Xunit;

namespace Minesweeper.Tests
{
    public class Method2
    {
        [Fact]
        public void SolveSecondCell()
        {
            var game = CreateGame();

            game.Solve();
            game.Solve();
            game.Solve();
            var actualSolution = game.Solve();

            Assert.Equal("All unknown cells must contain mines.", actualSolution.Description);
            Assert.Equal(0, actualSolution.CellOfInterest.Column);
            Assert.Equal(2, actualSolution.CellOfInterest.Row);

            Assert.Single(actualSolution.SolvedCells);
            Assert.Contains(actualSolution.SolvedCells, c => c.Column == 0 && c.Row == 3);

            var expectedBoard = "0x  1 11," +
                                "xx 3    ," +
                                "13 5 4  ," +
                                "!2   31x," +
                                "  332 x0," +
                                "  xxx1xx," +
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
