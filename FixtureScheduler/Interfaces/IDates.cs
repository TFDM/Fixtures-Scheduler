namespace Interfaces
{
    public interface IDates
    {
        public List<Models.AvailableDates> AvailableDates { get; }
        //List<Models.AvailableDates> GetAvailableDates();
        public void PrintDates();
        void AddBankHolidayDates();
        public bool MoreDatesRequired();
        public int TotalNumberOfAdditionalDatesRequired();
        int CountMatchdays(bool useAlternativeMatchday = false);
    }
}