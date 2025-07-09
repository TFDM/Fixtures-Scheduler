namespace Interfaces
{
    public interface IDates
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        DayOfWeek PrimaryMatchDay { get; }
        DayOfWeek AlternativeMatchDay { get; }
        List<Models.BankHolidayEvent>? BankHolidays { get; }

        int CountMatchdays(bool useAlternativeMatchday = false);
    }
}