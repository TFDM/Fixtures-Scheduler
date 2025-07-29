namespace Interfaces
{
    public interface IDates
    {
        public List<Models.BankHolidayEvent>? BankHolidays { get; }
        public List<Models.AvailableDates> AvailableDates { get; }
        List<Models.AvailableDateAddRemoveResult> AddPrimaryMatchDaysDates();
        List<Models.AvailableDateAddRemoveResult> AddBankHolidayDates(List<Models.BankHolidayEvent> bankHolidaysToAdded);
        public bool MoreDatesRequired();
        public int TotalNumberOfAdditionalDatesRequired();
    }
}