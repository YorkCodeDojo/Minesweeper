using System.Threading.Tasks;
using Xunit;

namespace Minesweeper.Tests
{
    public class CheckWeCanLoadGamesFromFiles
    {
        [Fact]
        public async Task CheckFilesCanBeLoaded()
        {
            var gameLoader = new LoadGameFromFile();
            var game = await gameLoader.Load("SimpleGame.txt");

            Assert.Equal(13, game.ExpectedMineCount);
            Assert.Equal(8, game.NumberOfColumns);
            Assert.Equal(8, game.NumberOfRows);
            Assert.Equal("0", game.CellContents(0, 0));
            Assert.Equal("", game.CellContents(1, 0));
            Assert.Equal("3", game.CellContents(1, 2));
        }
    }
}
