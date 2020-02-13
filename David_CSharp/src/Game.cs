using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper
{
    public class Game
    {
        public int NumberOfColumns { get; private set; }
        public int NumberOfRows { get; private set; }
        public int ExpectedMineCount { get; private set; }

        public const int ExpectedMineCountNotSpecified = -1;

        private const int UnknownCell = -1;
        private const int Mine = -2;
        private const int Empty = -3;

        private int[,] cells;

        public Game(char[,] initialState, int expectedMineCount)
        {
            ExpectedMineCount = expectedMineCount;
            NumberOfColumns = initialState.GetUpperBound(0) + 1;
            NumberOfRows = initialState.GetUpperBound(1) + 1;

            cells = new int[NumberOfColumns, NumberOfRows];

            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    cells[column, row] = initialState[column, row] switch
                    {
                        '!' => Mine,
                        'x' => Empty,
                        ' ' => UnknownCell,
                        '0' => 0,
                        '1' => 1,
                        '2' => 2,
                        '3' => 3,
                        '4' => 4,
                        '5' => 5,
                        '6' => 6,
                        '7' => 7,
                        '8' => 8,
                        _ => throw new Exception($"Cell {column},{row} contains {initialState[column, row]} which isn't valid."),
                    };
                }
            }

        }

        public bool BruteForce()
        {
            var gameValidator = new GameValidator();

            var nextUnknownCell = GetNextUnknownCell();
            if (nextUnknownCell.IsUnused) return true;

            cells[nextUnknownCell.Column, nextUnknownCell.Row] = Mine;
            if (gameValidator.Validate(this).IsValid)
            {
                var result = BruteForce();
                if (result) return true;
            }

            cells[nextUnknownCell.Column, nextUnknownCell.Row] = Empty;
            if (gameValidator.Validate(this).IsValid)
            {
                var result = BruteForce();
                if (result) return true;
            }

            cells[nextUnknownCell.Column, nextUnknownCell.Row] = UnknownCell;
            return false;
        }

        private CellLocation GetNextUnknownCell()
        {
            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (cells[column, row] == UnknownCell)
                    {
                        return new CellLocation(column, row);
                    }
                }
            }

            return CellLocation.NotUsed;    
        }

        public string CellContents(int column, int row)
        {
            return cells[column, row] switch
            {
                UnknownCell => "",
                Mine => "!",
                Empty => "x",
                _ => cells[column, row].ToString(),
            };
        }


        public Solution Solve()
        {
            var solution = Method1_NoMinesLeftToPlace();

            if (solution is null)
                solution = Method2_UnknownCellsMustBeMines();

            if (solution is null)
                solution = Method3_MustContainAMine();

            if (solution is null)
                solution = Method4_CannotContainAMine();

            if (solution is null)
                solution = Method5_MineCount();

            return solution;
        }

        private Solution Method5_MineCount()
        {
            if (ExpectedMineCount != ExpectedMineCountNotSpecified)
            {
                var minesPlaced = FindMines().Count;
                var remainingCells = FindUnknowns();
                var minesRemaining = ExpectedMineCount - minesPlaced;

                if (minesRemaining == 0 && remainingCells.Count > 0)
                {
                    foreach (var location in remainingCells)
                    {
                        cells[location.Column, location.Row] = Empty;
                    }

                    return new Solution
                    {
                        CellOfInterest = CellLocation.NotUsed,
                        SolvedCells = remainingCells.ToArray(),
                        Description = "All mines have been found.",
                    };
                }
                else if (minesRemaining > 0 && minesRemaining == remainingCells.Count)
                {
                    foreach (var location in remainingCells)
                    {
                        cells[location.Column, location.Row] = Mine;
                    }

                    return new Solution
                    {
                        CellOfInterest = CellLocation.NotUsed,
                        SolvedCells = remainingCells.ToArray(),
                        Description = "All remaining cells must be mines.",
                    };
                }

            }
            return null;
        }

        private Solution Method3_MustContainAMine()
        {
            var validator = new GameValidator();
            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (cells[column, row] == UnknownCell)
                    {
                        cells[column, row] = Empty;
                        var movesToUnDo = new List<CellLocation>();
                        var solution = default(Solution);
                        do
                        {
                            solution = Method1_NoMinesLeftToPlace();
                            if (solution is null)
                            {
                                solution = Method2_UnknownCellsMustBeMines();
                            }

                            if (solution is object)
                            {
                                movesToUnDo.AddRange(solution.SolvedCells);

                                var validationResult = validator.Validate(this);
                                if (!validationResult.IsValid)
                                {
                                    cells[column, row] = Mine;

                                    foreach (var moveToUndo in movesToUnDo)
                                    {
                                        cells[moveToUndo.Column, moveToUndo.Row] = UnknownCell;
                                    }

                                    return new Solution
                                    {
                                        CellOfInterest = validationResult.CellOfInterest,
                                        SolvedCells = new CellLocation[1] { new CellLocation(column, row) },
                                        Description = "Must contain a mine",
                                    };
                                }
                            }
                        } while (solution is object);

                        foreach (var moveToUndo in movesToUnDo)
                        {
                            cells[moveToUndo.Column, moveToUndo.Row] = UnknownCell;
                        }

                        cells[column, row] = UnknownCell;
                    }
                }
            }

            return null;
        }

        private Solution Method4_CannotContainAMine()
        {
            var validator = new GameValidator();
            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (cells[column, row] == UnknownCell)
                    {
                        cells[column, row] = Mine;
                        var movesToUnDo = new List<CellLocation>();
                        var solution = default(Solution);
                        do
                        {
                            solution = Method1_NoMinesLeftToPlace();
                            if (solution is null)
                            {
                                solution = Method2_UnknownCellsMustBeMines();
                            }

                            if (solution is object)
                            {
                                movesToUnDo.AddRange(solution.SolvedCells);

                                var validationResult = validator.Validate(this);
                                if (!validationResult.IsValid)
                                {
                                    cells[column, row] = Empty;

                                    foreach (var moveToUndo in movesToUnDo)
                                    {
                                        cells[moveToUndo.Column, moveToUndo.Row] = UnknownCell;
                                    }

                                    return new Solution
                                    {
                                        CellOfInterest = validationResult.CellOfInterest,
                                        SolvedCells = new CellLocation[1] { new CellLocation(column, row) },
                                        Description = "Cannot contain a mine",
                                    };
                                }
                            }
                        } while (solution is object);

                        foreach (var moveToUndo in movesToUnDo)
                        {
                            cells[moveToUndo.Column, moveToUndo.Row] = UnknownCell;
                        }

                        cells[column, row] = UnknownCell;
                    }
                }
            }

            return null;
        }

        private Solution Method1_NoMinesLeftToPlace()
        {
            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (TryGetMineCountForCell(column, row, out var mineCount))
                    {
                        var neighboursWithMines = NeighboursWithMines(column, row);
                        var neighboursWhichAreUnknown = NeighboursWhichAreUnknown(column, row);
                        var minesLeftToPlace = mineCount - (neighboursWithMines.Count());

                        if (minesLeftToPlace == 0 && neighboursWhichAreUnknown.Any())
                        {
                            foreach (var cell in neighboursWhichAreUnknown)
                            {
                                cells[cell.Column, cell.Row] = Empty;
                            }

                            return new Solution
                            {
                                CellOfInterest = new CellLocation(column, row),
                                SolvedCells = neighboursWhichAreUnknown,
                                Description = "No mines left to place.",
                            };
                        }
                    }
                }
            }

            return null;
        }


        private Solution Method2_UnknownCellsMustBeMines()
        {
            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (TryGetMineCountForCell(column, row, out var mineCount))
                    {
                        var neighboursWithMines = NeighboursWithMines(column, row);
                        var neighboursWhichAreUnknown = NeighboursWhichAreUnknown(column, row);
                        var minesLeftToPlace = mineCount - (neighboursWithMines.Count());

                        if (minesLeftToPlace > 0 && minesLeftToPlace == neighboursWhichAreUnknown.Count())
                        {
                            foreach (var cell in neighboursWhichAreUnknown)
                            {
                                cells[cell.Column, cell.Row] = Mine;
                            }

                            return new Solution
                            {
                                CellOfInterest = new CellLocation(column, row),
                                SolvedCells = neighboursWhichAreUnknown,
                                Description = "All unknown cells must contain mines.",
                            };
                        }
                    }
                }
            }

            return null;
        }

        private List<CellLocation> FindMines()
        {
            var result = new List<CellLocation>();
            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (cells[column, row] == Mine)
                    {
                        result.Add(new CellLocation(column, row));
                    }
                }
            }
            return result;
        }

        private List<CellLocation> FindUnknowns()
        {
            var result = new List<CellLocation>();
            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (cells[column, row] == UnknownCell)
                    {
                        result.Add(new CellLocation(column, row));
                    }
                }
            }
            return result;
        }

        internal CellLocation[] NeighboursWithMines(int column, int row)
        {
            var neighbours = GetNeighbours(column, row);
            return neighbours.Where(cell => cells[cell.Column, cell.Row] == Mine).ToArray();
        }

        internal CellLocation[] NeighboursWhichAreUnknown(int column, int row)
        {
            var neighbours = GetNeighbours(column, row);
            return neighbours.Where(cell => cells[cell.Column, cell.Row] == UnknownCell).ToArray();
        }

        private List<CellLocation> GetNeighbours(int column, int row)
        {
            var neighbours = new List<CellLocation>();

            /*
             *   123
             *   4 5
             *   678
             */

            if (column > 0 && row > 0) neighbours.Add(new CellLocation(column - 1, row - 1));
            if (row > 0) neighbours.Add(new CellLocation(column, row - 1));
            if (column < (NumberOfColumns - 1) && row > 0) neighbours.Add(new CellLocation(column + 1, row - 1));

            if (column > 0) neighbours.Add(new CellLocation(column - 1, row));
            if (column < (NumberOfColumns - 1)) neighbours.Add(new CellLocation(column + 1, row));

            if (column > 0 && row < (NumberOfRows - 1)) neighbours.Add(new CellLocation(column - 1, row + 1));
            if (row < (NumberOfRows - 1)) neighbours.Add(new CellLocation(column, row + 1));
            if (column < (NumberOfColumns - 1) && row < (NumberOfRows - 1)) neighbours.Add(new CellLocation(column + 1, row + 1));

            return neighbours;
        }

        internal bool TryGetMineCountForCell(int column, int row, out int mineCount)
        {
            if (cells[column, row] >= 0)
            {
                mineCount = cells[column, row];
                return true;
            }
            else
            {
                mineCount = int.MinValue;
                return false;
            }
        }
    }
}
