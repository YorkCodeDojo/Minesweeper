using System;
using Xunit;

namespace Minesweeper.Tests
{
    public class InitialStateOfGame
    {
        [Fact]
        public void CheckTheWidthAndHeightAreCorrectlyCalculated()
        {
            var initialState = new char[3, 4];
            InitialiseToUnknown(initialState);
            var game = new Game(initialState, Game.ExpectedMineCountNotSpecified);

            Assert.Equal(3, game.NumberOfColumns);
            Assert.Equal(4, game.NumberOfRows);
        }

        [Fact]
        public void CheckTheMineCountIsCorrect()
        {
            var initialState = new char[3, 4];
            InitialiseToUnknown(initialState);
            initialState[1, 2] = '8';
            initialState[2, 3] = '1';

            var game = new Game(initialState, Game.ExpectedMineCountNotSpecified);

            Assert.Equal("", game.CellContents(0, 0));
            Assert.Equal("8", game.CellContents(1, 2));
            Assert.Equal("1", game.CellContents(2, 3));
        }

        [Fact]
        public void CheckMinesAndEmptySquaresCanBeLoaded()
        {
            var initialState = new char[3, 4];
            InitialiseToUnknown(initialState);
            initialState[1, 2] = '!';
            initialState[2, 1] = 'x';

            var game = new Game(initialState, Game.ExpectedMineCountNotSpecified);

            Assert.Equal("!", game.CellContents(1, 2));
            Assert.Equal("x", game.CellContents(2, 1));
        }

        private void InitialiseToUnknown(char[,] initialState)
        {
            for (int column = 0; column <= initialState.GetUpperBound(0); column++)
            {
                for (int row = 0; row <= initialState.GetUpperBound(1); row++)
                {
                    initialState[column, row] = ' ';
                }
            }
        }
    }
}
