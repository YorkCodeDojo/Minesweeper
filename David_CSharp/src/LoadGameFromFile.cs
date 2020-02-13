using System.Threading.Tasks;

namespace Minesweeper
{
    public class LoadGameFromFile
    {
        public async Task<Game> Load(string filename)
        {
            var lines = await System.IO.File.ReadAllLinesAsync(filename).ConfigureAwait(false);
            var numberOfColumns = lines[0].Length;
            var numberOfRows = lines.Length;
            var mineCount = Game.ExpectedMineCountNotSpecified;

            if (lines[numberOfRows - 1].StartsWith("MineCount:"))
            {
                mineCount = int.Parse(lines[numberOfRows - 1]["MineCount:".Length..]);
                numberOfRows--;
            }

            var initialState = new char[numberOfColumns, numberOfRows];

            for (int column = 0; column < numberOfColumns; column++)
            {
                for (int row = 0; row < numberOfRows; row++)
                {
                    initialState[column, row] = lines[row][column];
                }
            }

            return new Game(initialState, mineCount);
        }
    }
}
