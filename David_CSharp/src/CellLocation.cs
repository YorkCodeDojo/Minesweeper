namespace Minesweeper
{
    public struct CellLocation
    {
        public int Column { get; private set; }
        public int Row { get; private set; }

        public CellLocation(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public static CellLocation NotUsed = new CellLocation(-1, -1);

        public bool IsUnused => Column == -1;
    }
}
