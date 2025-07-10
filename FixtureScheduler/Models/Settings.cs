namespace Models
{
	public class Settings
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DayOfWeek PrimaryMatchDay { get; set; }
		public DayOfWeek AlternativeMatchday { get; set; }
	}
}