namespace Models
{
	public class Settings
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DayOfWeek PrimaryMatchDay { get; set; }
		public DayOfWeek AlternativeMatchday { get; set; }
		public List<ExcludedDates> ExcludedDates { get; set; } = new List<ExcludedDates>();
	}

	public class ExcludedDates
	{
		public DateTime Date { get; set; }
		public string Reason { get; set; }
	}
}