namespace Minesweeper
{
    public partial class GameValidator
    {
        public ValidationResult Validate(Game game)
        {
            for (int column = 0; column < game.NumberOfColumns; column++)
            {
                for (int row = 0; row < game.NumberOfRows; row++)
                {
                    if (game.TryGetMineCountForCell(column, row, out var mineCount))
                    {
                        var neighboursWithMines = game.NeighboursWithMines(column, row).Length;

                        if (neighboursWithMines > mineCount)
                        {
                            return new ValidationResult
                            {
                                CellOfInterest = new CellLocation(column, row),
                                IsValid = false,
                                Reason = "Too many mines",
                            };
                        }
                    }
                }
            }

            for (int column = 0; column < game.NumberOfColumns; column++)
            {
                for (int row = 0; row < game.NumberOfRows; row++)
                {
                    if (game.TryGetMineCountForCell(column, row, out var mineCount))
                    {
                        var neighboursWithMines = game.NeighboursWithMines(column, row).Length;
                        var neighboursWhichAreUnknown = game.NeighboursWhichAreUnknown(column, row).Length;


                        if (mineCount > (neighboursWithMines + neighboursWhichAreUnknown))
                        {
                            return new ValidationResult
                            {
                                CellOfInterest = new CellLocation(column, row),
                                IsValid = false,
                                Reason = "Too few mines",
                            };
                        }
                    }
                }
            }

            return new ValidationResult
            {
                IsValid = true,
            };

        }
    }
}
