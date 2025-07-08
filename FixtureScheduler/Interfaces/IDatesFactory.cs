namespace Interfaces
{
    public interface IDatesFactory
    {
        IDates Create(DateTime startDate, DateTime endDate, DayOfWeek primaryMatchday, DayOfWeek altMatchday);
    }
}
