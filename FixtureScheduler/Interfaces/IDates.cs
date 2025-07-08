namespace Interfaces
{
    public interface IDates
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        DayOfWeek PrimaryMatchDay { get; set; }
        DayOfWeek AlternativeMatchDay { get; set; }
        List<Models.BankHolidayEvent>? BankHolidays { get; set; }

        int CountMatchdays(bool useAlternativeMatchday = false);
    }
}