using Models;

namespace BusinessLogic
{
    public class Dates : Interfaces.IDates
    {
        private readonly Interfaces.IApplicationSettings _applicationSettings;
        private readonly Interfaces.IBankHolidays _bankHolidays;
        private List<Models.BankHolidayEvent>? BankHolidays { get; set; }
        public List<Models.AvailableDates> AvailableDates { get; private set; }
        
        public Dates(Interfaces.IApplicationSettings applicationSettings, Interfaces.IBankHolidays bankHolidays)
        {
            //Application settings instance is passed in via dependency injection
            _applicationSettings = applicationSettings;

            //Bank holidays instance is passed in via dependency injection
            _bankHolidays = bankHolidays;

            //Gets bank holidays between the supplied dates
            this.BankHolidays = _bankHolidays.GetBankHolidays(_applicationSettings.Settings.StartDate,
                _applicationSettings.Settings.EndDate);

            //Gets the available dates
            this.AvailableDates = GetAvailableDates();
        }

        /// <summary>
        /// Generates a list of available dates
        /// </summary>
        /// <returns></returns>
        private List<Models.AvailableDates> GetAvailableDates()
        {
            //Creates a new list of available dates
            var availableDates = new List<Models.AvailableDates>();

            //Adds the bank holiday dates - takes into account any excluded dates as well
            availableDates = AddBankHolidayDates(availableDates);

            //Sets the current date variable to the start date
            var currentDate = _applicationSettings.Settings.StartDate;
            
            //Keep looping while the current date is less than or equal to the end date
            while (currentDate <= _applicationSettings.Settings.EndDate)
            {
                //Checks if the current date's day of the week is equal to the primary matchday and the current 
                //date is not equal to any of the dates in the excluded dates list
                if (currentDate.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay && !_applicationSettings.Settings.ExcludedDates.Any(x => x.Date == currentDate))
                {
                    //Checks to make sure that there isn't already a date in the list
                    //which is within 1 day in either direction
                    if (!availableDates.Any(x => x.Date == currentDate.AddDays(-1)) &&
                        !availableDates.Any(x => x.Date == currentDate.AddDays(1)))
                    {
                        //Add the current date to the list of available dates
                        availableDates.Add(new Models.AvailableDates
                        {
                            Date = currentDate,
                            IsPrimaryMatchday = (currentDate.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay) ? true : false
                        });
                    }
                }

                //Add a day to the current date
                currentDate = currentDate.AddDays(1);
            }

            return availableDates;
        }

        /// <summary>
        /// Adds bank holidays to a list of available dates
        /// </summary>
        /// <param name="listOfAvailableDates"></param>
        /// <returns></returns>
        private List<Models.AvailableDates> AddBankHolidayDates(List<Models.AvailableDates> listOfAvailableDates)
        {
            if (this.BankHolidays != null)
            {
                foreach (var bankHoliday in this.BankHolidays)
                {
                    //Skips over the current bank holiday in the loop if its one of excluded dates in the application
                    //Otheriwse check to make sure the bank holiday
                    if (!_applicationSettings.Settings.ExcludedDates.Any(x => x.Date == bankHoliday.Date))
                    {
                        //Checks to make sure that there isn't already a date in the list
                        //which is within 1 day in either direction
                        if (!listOfAvailableDates.Any(x => x.Date == bankHoliday.Date.AddDays(-1)) &&
                            !listOfAvailableDates.Any(x => x.Date == bankHoliday.Date.AddDays(1)))
                        {
                            listOfAvailableDates.Add(new Models.AvailableDates
                            {
                                Date = bankHoliday.Date,
                                IsPrimaryMatchday = (bankHoliday.Date.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay) ? true : false
                            });
                        }
                    }
                }
            }

            return listOfAvailableDates;
        }

        public void PrintDates()
        {
            foreach (var availableDate in this.AvailableDates.OrderBy(x => x.Date))
            {
                Console.WriteLine(availableDate.Date.ToString("dd/MM/yyyy") + "is a " + availableDate.Date.DayOfWeek.ToString());
            }
        }

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
            var currentDate = _applicationSettings.Settings.StartDate;

            //Keeps loop over the dates until the end date is reached
            while (currentDate <= _applicationSettings.Settings.EndDate)
            {
                //If the current day in the loop matches either the alternative or primary matchday
                //the count is increased by one
                if (currentDate.DayOfWeek == ((useAlternativeMatchday) ? _applicationSettings.Settings.AlternativeMatchday : _applicationSettings.Settings.PrimaryMatchDay))
                {
                    count = count + 1;
                    Console.WriteLine(
                        currentDate.ToString("dd/MM/yyyy") + " is a " +
                        ((useAlternativeMatchday) ? _applicationSettings.Settings.AlternativeMatchday.ToString() : _applicationSettings.Settings.PrimaryMatchDay.ToString())
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