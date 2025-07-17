namespace Interfaces
{
    public interface IDates
    {
        List<Models.AvailableDates> GetAvailableDates();

        int CountMatchdays(bool useAlternativeMatchday = false);
    }
}