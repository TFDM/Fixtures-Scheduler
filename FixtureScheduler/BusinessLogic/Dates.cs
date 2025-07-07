namespace BusinessLogic
{
    public class Dates : Interfaces.IDates
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public int CountSaturdays()
        {
            return 0;
        }
    }
}