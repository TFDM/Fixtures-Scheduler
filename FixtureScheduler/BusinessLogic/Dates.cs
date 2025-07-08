namespace BusinessLogic
{
    public class Dates : Interfaces.IDates
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DayOfWeek PrimaryMatchDay { get; set; }
        public DayOfWeek AlternativeMatchDay { get; set; }
        public List<Models.BankHolidayEvent>? BankHolidays { get; set; }

        /// <summary>
        /// Counts the number of primary or alternative matchdays
        /// </summary>
        /// <param name="useAlternativeMatchday"></param>
        /// <returns></returns>
        public int CountMatchdays(bool useAlternativeMatchday = false)
        {
            //Sets a count to be used in the loop below
            var count = 0;

            //Sets a variable to the start date - this is used to advance the loop below
            var currentDate = this.StartDate;

            //Keeps loop over the dates until the end date is reached
            while (currentDate <= this.EndDate)
            {
                //If the current day in the loop matches either the alternative or primary matchday
                //the count is increased by one
                if (currentDate.DayOfWeek == ((useAlternativeMatchday) ? this.AlternativeMatchDay : this.PrimaryMatchDay))
                {
                    count = count + 1;
                    Console.WriteLine(
                        currentDate.ToString("dd/MM/yyyy") + " is a " +
                        ((useAlternativeMatchday) ? this.AlternativeMatchDay.ToString() : this.PrimaryMatchDay.ToString())
                        );
                }

                //Adds one day to the current date to advance the loop
                currentDate = currentDate.AddDays(1);
            }

            //Return the count of matchdays
            return count;
        }
    }
}