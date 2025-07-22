namespace Interfaces
{
    public interface IDates
    {
        public List<Models.AvailableDates> AvailableDates { get; }
        //List<Models.AvailableDates> GetAvailableDates();
        public void PrintDates();

        int CountMatchdays(bool useAlternativeMatchday = false);
    }
}