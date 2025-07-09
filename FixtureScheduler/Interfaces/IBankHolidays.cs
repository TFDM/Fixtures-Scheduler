namespace Interfaces
{
    public interface IBankHolidays
    {
        List<Models.BankHolidayEvent>? GetBankHolidays(DateTime startDate, DateTime endDate);
    }
}