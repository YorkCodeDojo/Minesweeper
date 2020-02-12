namespace Minesweeper
{
    public partial class GameValidator
    {
        public class ValidationResult
        {
            public bool IsValid { get; set; }

            public string Reason { get; set; }

            public CellLocation CellOfInterest { get; set; }
        }
    }
}
