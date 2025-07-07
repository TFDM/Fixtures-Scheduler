namespace BusinessLogic
{
    public class DateFactory : Interfaces.IDatesFactory
    {
        public Interfaces.IDates Create(DateTime startDate, DateTime endDate)
        {
            return new Dates
            {
                StartDate = startDate,
                EndDate = endDate
            };
        }
    }
}