namespace Interfaces
{
    public interface IDates
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }

        int CountSaturdays();
    }
}