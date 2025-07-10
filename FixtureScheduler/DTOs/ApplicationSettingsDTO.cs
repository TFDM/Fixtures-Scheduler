namespace DTOs
{
    public class ApplicationSettingsDTO
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string PrimaryMatchday { get; set; } = string.Empty;
        public string AlternativeMatchday { get; set; } = string.Empty;
        public List<ExcludedDatesDTO> ExcludedDates { get; set; } = new List<ExcludedDatesDTO>();
    }

    public class ExcludedDatesDTO
    {
        public string Date { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}