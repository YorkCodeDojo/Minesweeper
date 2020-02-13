using System.Drawing;

namespace Minesweeper
{
    class GameDrawer
    {
        private const int CellSize = 50;
        private readonly Font _symbolFont = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        private readonly Font _infoFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        private readonly Pen _borderPen = new Pen(Brushes.Black, 3);
        private Game _game;

        public GameDrawer(Game game)
        {
            _game = game;
        }

        public void Draw(Graphics g)
        {
            g.TranslateTransform(100, 100);

            g.DrawRectangle(_borderPen, 0, 0, _game.NumberOfColumns * CellSize, _game.NumberOfRows * CellSize);

            for (int column = 1; column < _game.NumberOfColumns; column++)
            {
                g.DrawLine(Pens.Black, column * CellSize, 0, column * CellSize, _game.NumberOfRows * CellSize);
            }

            for (int row = 1; row < _game.NumberOfRows; row++)
            {
                g.DrawLine(Pens.Black, 0, row * CellSize, _game.NumberOfColumns * CellSize, row * CellSize);
            }

            for (int column = 0; column < _game.NumberOfColumns; column++)
            {
                for (int row = 0; row < _game.NumberOfRows; row++)
                {
                    DrawSymbol(g, _game.CellContents(column, row), column, row, Brushes.Black);
                }
            }
        }

        internal void DrawMove(Graphics g, Solution result)
        {
            if (!result.CellOfInterest.IsUnused)
            {
                DrawSymbol(g, _game.CellContents(result.CellOfInterest.Column, result.CellOfInterest.Row), result.CellOfInterest.Column, result.CellOfInterest.Row, Brushes.Blue);
            }

            foreach (var cell in result.SolvedCells)
            {
                DrawSymbol(g, _game.CellContents(cell.Column, cell.Row), cell.Column, cell.Row, Brushes.Red);
            }

            g.DrawString(result.Description, _infoFont, Brushes.Black, (_game.NumberOfColumns + 1) * CellSize, (result.CellOfInterest.Row * CellSize) + 20);

        }

        private void DrawSymbol(Graphics g, string symbol, int column, int row, Brush brush)
        {
            var x = column * CellSize;
            var y = row * CellSize;

            if (symbol == "x")
            {
                symbol = "-";
            }

            if (symbol == "!")
            {
                g.FillEllipse(brush, x + (CellSize / 4), y + (CellSize / 4), CellSize / 2, CellSize / 2);
            }
            else
            {
                var symbolSize = g.MeasureString(symbol, _symbolFont, new Point(x, y), StringFormat.GenericDefault);
                g.DrawString(symbol, _symbolFont, brush, x + ((CellSize - symbolSize.Width) / 2), y + ((CellSize - symbolSize.Height) / 2));
            }
        }
    }
}
