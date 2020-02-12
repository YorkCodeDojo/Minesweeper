using Xunit;

namespace Minesweeper.Tests
{
    public class ValidateGameTests
    {
        [Fact]
        public void TooManyMines()
        {
            var details = "3   1 11," +
                          "!! 3    ," +
                          "13 5 4  ," +
                          " 2   31 ," +
                          "  332  0," +
                          "     1  ," +
                          " 1 0 1  ," +
                          "        ";

            var game = CreateGame(details);
            var validator = new GameValidator();

            var validationResult = validator.Validate(game);

            Assert.False(validationResult.IsValid);
            Assert.Equal("Too many mines", validationResult.Reason);
            Assert.Equal(0, validationResult.CellOfInterest.Column);
            Assert.Equal(2, validationResult.CellOfInterest.Row);
        }


        [Fact]
        public void CannotPlaceAllMines()
        {
            var details = "0   1 11," +
                          "   3    ," +
                          "13 8 4  ," +
                          " 2   31 ," +
                          "  332  0," +
                          "     1  ," +
                          " 1 0 1  ," +
                          "        ";

            var game = CreateGame(details);
            var validator = new GameValidator();

            var validationResult = validator.Validate(game);

            Assert.False(validationResult.IsValid);
            Assert.Equal("Too few mines", validationResult.Reason);
            Assert.Equal(3, validationResult.CellOfInterest.Column);
            Assert.Equal(2, validationResult.CellOfInterest.Row);
        }

        [Fact]
        public void ValidBoard()
        {
            var details = "0   1 11," +
                          "   3    ," +
                          "13 5 4  ," +
                          " 2   31 ," +
                          "  332  0," +
                          "     1  ," +
                          " 1 0 1  ," +
                          "        ";

            var game = CreateGame(details);
            var validator = new GameValidator();

            var validationResult = validator.Validate(game);

            Assert.True(validationResult.IsValid);
        }


        private Game CreateGame(string details)
        {
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
