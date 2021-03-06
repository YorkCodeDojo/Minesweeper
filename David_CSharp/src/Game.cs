﻿using System;
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

#pragma warning disable IDE1006 // Naming Styles
        private const int UnknownCell = -1;
        private const int Mine = -2;
        private const int Empty = -3;
#pragma warning restore IDE1006 // Naming Styles

        private int[,] _cells;

        public Game(char[,] initialState, int expectedMineCount)
        {
            ExpectedMineCount = expectedMineCount;
            NumberOfColumns = initialState.GetUpperBound(0) + 1;
            NumberOfRows = initialState.GetUpperBound(1) + 1;

            _cells = new int[NumberOfColumns, NumberOfRows];

            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    _cells[column, row] = initialState[column, row] switch
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

        public int CleverBruteForce()
        {
            var numberOfWrongMoves = 0;

            bool Work()
            {
                var gameValidator = new GameValidator();

                var nextUnknownCell = GetMostLikelyUnknownCell();
                if (nextUnknownCell.IsUnused) return true;

                var probabilty = CalculateProbability(nextUnknownCell.Column, nextUnknownCell.Row);

                _cells[nextUnknownCell.Column, nextUnknownCell.Row] = probabilty > 0.5M ? Mine : Empty;
                if (gameValidator.Validate(this).IsValid)
                {
                    var result = Work();
                    if (result) return true;
                }
                else
                {
                    numberOfWrongMoves++;
                }

                _cells[nextUnknownCell.Column, nextUnknownCell.Row] = probabilty > 0.5M ? Empty : Mine;
                if (gameValidator.Validate(this).IsValid)
                {
                    var result = Work();
                    if (result) return true;
                }
                else
                {
                    numberOfWrongMoves++;
                }

                _cells[nextUnknownCell.Column, nextUnknownCell.Row] = UnknownCell;
                return false;
            }

            Work();

            return numberOfWrongMoves;
        }

        private CellLocation GetMostLikelyUnknownCell()
        {
            var bestProbability = 0.0M;
            var bestLocation = CellLocation.NotUsed;

            var rnd = new Random();

            if (rnd.Next(0, 2) == 0)
            {
                for (int column = NumberOfColumns - 1; column >= 0; column--)
                {
                    for (int row = NumberOfRows - 1; row >= 0; row--)
                    {
                        if (_cells[column, row] == UnknownCell)
                        {
                            var probabilty = CalculateProbability(column, row);
                            if (probabilty == 0 || probabilty == 1)
                            {
                                return new CellLocation(column, row);
                            }

                            probabilty = Math.Abs(probabilty - 0.5M);

                            if (probabilty > (bestProbability)) // + (rnd.Next(0, 3) / 10.0M)))
                            {
                                bestProbability = probabilty;
                                bestLocation = new CellLocation(column, row);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int column = 0; column < NumberOfColumns; column++)
                {
                    for (int row = 0; row < NumberOfRows; row++)
                    {
                        if (_cells[column, row] == UnknownCell)
                        {
                            var probabilty = CalculateProbability(column, row);
                            if (probabilty == 0 || probabilty == 1)
                            {
                                return new CellLocation(column, row);
                            }

                            probabilty = Math.Abs(probabilty - 0.5M);

                            if (probabilty > (bestProbability))// + (rnd.Next(0, 3) / 10.0M)))
                            {
                                bestProbability = probabilty;
                                bestLocation = new CellLocation(column, row);
                            }
                        }
                    }
                }
            }

            return bestLocation;
        }

        public int BruteForce()
        {
            var numberOfWrongMoves = 0;

            bool Work()
            {
                var gameValidator = new GameValidator();

                var nextUnknownCell = GetNextUnknownCell();
                if (nextUnknownCell.IsUnused) return true;

                _cells[nextUnknownCell.Column, nextUnknownCell.Row] = Mine;
                if (gameValidator.Validate(this).IsValid)
                {
                    var result = Work();
                    if (result) return true;
                }
                else
                {
                    numberOfWrongMoves++;
                }

                _cells[nextUnknownCell.Column, nextUnknownCell.Row] = Empty;
                if (gameValidator.Validate(this).IsValid)
                {
                    var result = Work();
                    if (result) return true;
                }
                else
                {
                    numberOfWrongMoves++;
                }

                _cells[nextUnknownCell.Column, nextUnknownCell.Row] = UnknownCell;
                return false;
            }

            Work();

            return numberOfWrongMoves;
        }



        private CellLocation GetNextUnknownCell()
        {
            for (int column = 0; column < NumberOfColumns; column++)
            {
                for (int row = 0; row < NumberOfRows; row++)
                {
                    if (_cells[column, row] == UnknownCell)
                    {
                        return new CellLocation(column, row);
                    }
                }
            }

            return CellLocation.NotUsed;
        }

        internal decimal CalculateProbability(int column, int row)
        {
            /*Return a value between 0 and 1
             * 
             * 0 means cannot contain a mine
             * 
             * 0.5 means we don't know
             * 
             * 1 must contain a mine
             */
            var total = 0.0M;
            var number = 0;

            var neighbours = GetNeighbours(column, row);
            foreach (var neighbour in neighbours)
            {
                if (TryGetMineCountForCell(neighbour.Column, neighbour.Row, out var mineCount))
                {
                    var neighboursWithMines = NeighboursWithMines(neighbour.Column, neighbour.Row).Count();
                    var neighboursWhichAreUnknown = NeighboursWhichAreUnknown(neighbour.Column, neighbour.Row).Count();
                    var minesLeftToPlace = mineCount - neighboursWithMines;

                    if (minesLeftToPlace == 0)
                        return 0;

                    if (minesLeftToPlace == neighboursWhichAreUnknown)
                        return 1;

                    total += minesLeftToPlace / (decimal)neighboursWhichAreUnknown;
                    number++;
                }
            }

            if (number == 0)
                return 0.5M;
            else
                return Math.Round(total / number, 2);
        }

        public string CellContents(int column, int row)
        {
            return _cells[column, row] switch
            {
                UnknownCell => "",
                Mine => "!",
                Empty => "x",
                _ => _cells[column, row].ToString(),
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
                        _cells[location.Column, location.Row] = Empty;
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
                        _cells[location.Column, location.Row] = Mine;
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
                    if (_cells[column, row] == UnknownCell)
                    {
                        _cells[column, row] = Empty;
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
                                    _cells[column, row] = Mine;

                                    foreach (var moveToUndo in movesToUnDo)
                                    {
                                        _cells[moveToUndo.Column, moveToUndo.Row] = UnknownCell;
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
                            _cells[moveToUndo.Column, moveToUndo.Row] = UnknownCell;
                        }

                        _cells[column, row] = UnknownCell;
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
                    if (_cells[column, row] == UnknownCell)
                    {
                        _cells[column, row] = Mine;
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
                                    _cells[column, row] = Empty;

                                    foreach (var moveToUndo in movesToUnDo)
                                    {
                                        _cells[moveToUndo.Column, moveToUndo.Row] = UnknownCell;
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
                            _cells[moveToUndo.Column, moveToUndo.Row] = UnknownCell;
                        }

                        _cells[column, row] = UnknownCell;
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
                                _cells[cell.Column, cell.Row] = Empty;
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
                                _cells[cell.Column, cell.Row] = Mine;
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
                    if (_cells[column, row] == Mine)
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
                    if (_cells[column, row] == UnknownCell)
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
            return neighbours.Where(cell => _cells[cell.Column, cell.Row] == Mine).ToArray();
        }

        internal CellLocation[] NeighboursWhichAreUnknown(int column, int row)
        {
            var neighbours = GetNeighbours(column, row);
            return neighbours.Where(cell => _cells[cell.Column, cell.Row] == UnknownCell).ToArray();
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
            if (_cells[column, row] >= 0)
            {
                mineCount = _cells[column, row];
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
