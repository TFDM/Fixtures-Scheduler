using System.Text.Json.Serialization;

namespace Models
{
    public class BankHolidays
    {
        [JsonPropertyName("england-and-wales")]
        public Division? EnglandAndWales { get; set; }

        [JsonPropertyName("scotland")]
        public Division? Scotland { get; set; }

        [JsonPropertyName("northern-ireland")]
        public Division? NorthernIreland { get; set; }
    }

    public class Division
    {
        [JsonPropertyName("division")]
        public string? DivisionName { get; set; }

        [JsonPropertyName("events")]
        public List<BankHolidayEvent> Events { get; set; } = new List<BankHolidayEvent>();
    }

    public class BankHolidayEvent
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("date")]
        public string? DateString { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("bunting")]
        public bool Bunting { get; set; }

        // Computed property to get the date as DateTime
        [JsonIgnore]
        public DateTime Date => DateTime.Parse(DateString!);

        // Helper property to check if it's a substitute day
        [JsonIgnore]
        public bool IsSubstituteDay => !string.IsNullOrEmpty(Notes) && Notes.Contains("Substitute day");
    }

    public static class BankHolidayExtensions
    {
        public static IEnumerable<BankHolidayEvent> GetEventsForYear(this Division division, int year)
        {
            return division.Events.Where(e => DateTime.Parse(e.DateString).Year == year);
        }

        public static IEnumerable<BankHolidayEvent> GetEventsByTitle(this Division division, string title)
        {
            return division.Events.Where(e => e.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<BankHolidayEvent> GetBuntingEvents(this Division division)
        {
            return division.Events.Where(e => e.Bunting);
        }
    }

}