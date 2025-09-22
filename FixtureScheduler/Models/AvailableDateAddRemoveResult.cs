namespace Models
{
    public class AvailableDateAddRemoveResult
    {
        public DateTime Date { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}