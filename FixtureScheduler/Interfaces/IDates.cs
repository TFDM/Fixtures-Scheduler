namespace Interfaces
{
    public interface IDates
    {
        public List<Models.BankHolidayEvent>? BankHolidays { get; }
        public List<Models.AvailableDates> AvailableDates { get; }
        public List<Models.AvailableDates> PrimaryDates { get; }
        public List<Models.AvailableDates> AlternativeDates { get; }
        List<Models.AvailableDateAddRemoveResult> AddSelectedDates(List<Models.AvailableDates> datesToBeAdded);
        List<Models.AvailableDateAddRemoveResult> AddAllPrimaryMatchDaysDates();
        List<Models.AvailableDateAddRemoveResult> AddBankHolidayDates(List<Models.BankHolidayEvent> bankHolidaysToAdded);
        public bool MoreDatesRequired();
        public int TotalNumberOfAdditionalDatesRequired();
    }
}