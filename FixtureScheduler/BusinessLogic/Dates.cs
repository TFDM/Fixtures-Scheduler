using Models;
using Spectre.Console;

namespace BusinessLogic
{
    public class Dates : Interfaces.IDates
    {
        private readonly Interfaces.IApplicationSettings _applicationSettings;
        private readonly Interfaces.IBankHolidays _bankHolidays;
        public List<Models.BankHolidayEvent>? BankHolidays { get; private set; }
        public List<Models.AvailableDates> AvailableDates { get; private set; } = new List<Models.AvailableDates>();
        
        public Dates(Interfaces.IApplicationSettings applicationSettings,
            Interfaces.IBankHolidays bankHolidays)
        {
            // Application settings instance is passed in via dependency injection
            _applicationSettings = applicationSettings;

            // Bank holidays instance is passed in via dependency injection
            _bankHolidays = bankHolidays;

            // Gets bank holidays between the supplied dates
            this.BankHolidays = _bankHolidays.GetBankHolidays(_applicationSettings.Settings.StartDate,
                _applicationSettings.Settings.EndDate);
        }

        /// <summary>
        /// Adds primary match days to the list of available dates using the settings.
        /// Takes into account any excluded dates
        /// </summary>
        /// <returns></returns>
        public List<Models.AvailableDateAddRemoveResult> AddPrimaryMatchDaysDates()
        {
            // Creates a list of results keep track of the added / skipped dates
            List<Models.AvailableDateAddRemoveResult> results = new List<Models.AvailableDateAddRemoveResult>();

            // Sets the current date variable to the start date from the settings
            var currentDate = _applicationSettings.Settings.StartDate;

            //Keep looping while the current date is less than or equal to the end date
            while (currentDate <= _applicationSettings.Settings.EndDate)
            {
                // If the current date's day of the week doesn't match the primary match day then skip it
                if (currentDate.DayOfWeek != _applicationSettings.Settings.PrimaryMatchDay)
                {
                    // Add a day to the current date
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                // If the current date matches any of the excluded dates then skip it
                if (_applicationSettings.Settings.ExcludedDates.Any(x => x.Date == currentDate))
                {
                    results.Add(new AvailableDateAddRemoveResult
                    {
                        Date = currentDate,
                        Action = "Skipped",
                        Reason = "Date matches an excluded date"
                    });

                    // Add a day to the current date
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                // If the current date is within 1 day of an exisiting avaiable date then skip it
                if (this.AvailableDates.Any(x => x.Date == currentDate.AddDays(-1)) ||
                    this.AvailableDates.Any(x => x.Date == currentDate.AddDays(1)))
                {
                    results.Add(new AvailableDateAddRemoveResult
                    {
                        Date = currentDate,
                        Action = "Skipped",
                        Reason = "Date is too close to an exisiting matchday"
                    });

                    // Add a day to the current date
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                // If the current date matches any of the exisiting available dates then skip it
                if (this.AvailableDates.Any(x => x.Date == currentDate))
                {
                    results.Add(new AvailableDateAddRemoveResult
                    {
                        Date = currentDate,
                        Action = "Skipped",
                        Reason = "Date is in already in the available dates."
                    });

                    // Add a day to the current date
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                // None of the previous conditions have been met - add the current date
                // to the list of available dates
                this.AvailableDates.Add(new Models.AvailableDates
                {
                    Date = currentDate,
                    IsPrimaryMatchday = (currentDate.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay) ? true : false
                });

                // Add the current date to the list of added dates
                results.Add(new AvailableDateAddRemoveResult
                {
                    Date = currentDate,
                    Action = "Added",
                    Reason = "Date added"
                });

                //Add a day to the current date
                currentDate = currentDate.AddDays(1);
            }

            // Returns the added dates
            return results;
        }

        /// <summary>
        /// Allows the user to add bank holidays, taking into account 
        /// any excluded dates from the settings
        /// </summary>
        public List<Models.AvailableDateAddRemoveResult> AddBankHolidayDates(List<Models.BankHolidayEvent> bankHolidaysToAdded)
        {
            // Creates a list of results keep track of the added / skipped dates
            List<Models.AvailableDateAddRemoveResult> results = new List<Models.AvailableDateAddRemoveResult>();

            // Adds the bank holiday to the list of available days
            foreach (var bankHoliday in bankHolidaysToAdded)
            {
                // If the bank holiday is already in the available dates then skip it
                if (this.AvailableDates.Any(x => x.Date == bankHoliday.Date))
                {
                    results.Add(new AvailableDateAddRemoveResult
                    {
                        Date = bankHoliday.Date,
                        Action = "Skipped",
                        Reason = $"{bankHoliday.Date:dd/MM/yyyy} ({bankHoliday.Title}) is already in the available dates."
                    });

                    continue;
                }

                // If the bank holiday matches any of the excluded dates then skip it
                if (_applicationSettings.Settings.ExcludedDates.Any(x => x.Date == bankHoliday.Date))
                {
                    results.Add(new AvailableDateAddRemoveResult
                    {
                        Date = bankHoliday.Date,
                        Action = "Skipped",
                        Reason = $"{bankHoliday.Date:dd/MM/yyyy} ({bankHoliday.Title}) is an excluded date."
                    });

                    continue;
                }

                // If the bank holiday is within 1 day of an exisiting avaiable date then skip it
                if (this.AvailableDates.Any(x => x.Date == bankHoliday.Date.AddDays(-1)) ||
                    this.AvailableDates.Any(x => x.Date == bankHoliday.Date.AddDays(1)))
                {
                    results.Add(new AvailableDateAddRemoveResult
                    {
                        Date = bankHoliday.Date,
                        Action = "Skipped",
                        Reason = $"{bankHoliday.Date:dd/MM/yyyy} ({bankHoliday.Title}) is too close to an exisiting matchday."
                    });

                    continue;
                }

                // None of the previous conditions have been met - add the bank holiday
                // to the list of available dates
                this.AvailableDates.Add(new Models.AvailableDates
                {
                    Date = bankHoliday.Date,
                    IsPrimaryMatchday = (bankHoliday.Date.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay) ? true : false
                });

                // Bank holiday was added
                results.Add(new AvailableDateAddRemoveResult
                {
                    Date = bankHoliday.Date,
                    Action = "Added",
                    Reason = $"{bankHoliday.Date:dd/MM/yyyy} ({bankHoliday.Title}) was added."
                });
            }

            return results;
        }

        /// <summary>
        /// Returns either true or false based on if more dates are are required
        /// compared to the number of rounds required
        /// </summary>
        /// <returns>bool</returns>
        public bool MoreDatesRequired()
        {
            return (this.AvailableDates.Count() < _applicationSettings.Settings.NumberOfRoundsNeeded) ? true : false;
        }

        /// <summary>
        /// Returns the total number of additional dates that are required 
        /// in order to fulfill the number of rounds required
        /// </summary>
        /// <returns></returns>
        public int TotalNumberOfAdditionalDatesRequired()
        {
            return _applicationSettings.Settings.NumberOfRoundsNeeded - this.AvailableDates.Count();
        }
    }
}