using Models;
using Spectre.Console;

namespace BusinessLogic
{
    public class Dates : Interfaces.IDates
    {
        private readonly Interfaces.IApplicationSettings _applicationSettings;
        private readonly Interfaces.IBankHolidays _bankHolidays;
        public List<Models.BankHolidayEvent>? BankHolidays { get; private set; }
        public List<Models.AvailableDates> PrimaryDates { get; private set; } = new List<Models.AvailableDates>();
        public List<Models.AvailableDates> AlternativeDates { get; private set; } = new List<Models.AvailableDates>();
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

            // Gets the primary match dates
            this.PrimaryDates = GetPrimaryDates();

            // Gets the alternative match dates
            this.AlternativeDates = GetAlternativeDates();
        }

        /// <summary>
        /// Validates the date to see if its already been used, is an excluded date 
        /// or is too close to an exisiting date. Returns either a reason or null
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private string? ValidateDate(DateTime d)
        {
            if (IsDateAlreadyUsed(d))
                return $"has already been added";

            if (IsDateExcludedDate(d))
                return $"is an excluded date";

            if (IsTooCloseToExisitingDate(d))
                return $"is too close to an exisiting date";

            return null; // no problems
        }

        /// <summary>
        /// Checks if a date is already in the list of available dates
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private bool IsDateAlreadyUsed(DateTime d)
        {
            // If the date is already in the available dates then it can't be added
            if (this.AvailableDates.Any(x => x.Date == d.Date))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a date matches any of the excluded dates in the settings
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private bool IsDateExcludedDate(DateTime d)
        {
            // If the date matches any of the excluded dates then it can't be added
            if (_applicationSettings.Settings.ExcludedDates.Any(x => x.Date == d.Date))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a date is too close to a date already in the list of available dates
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private bool IsTooCloseToExisitingDate(DateTime d)
        {
            // If the date is within 1 day of an exisiting avaiable date then it can't be added
            if (this.AvailableDates.Any(x => x.Date == d.Date.AddDays(-1)) ||
                    this.AvailableDates.Any(x => x.Date == d.Date.AddDays(1)))
                return true;
            return false;
        }

        /// <summary>
        /// Creates an AvailableDateAddRemoveResult
        /// </summary>
        /// <param name="date"></param>
        /// <param name="action"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        private AvailableDateAddRemoveResult CreateResult(DateTime date, string action, string reason)
        {
            return new AvailableDateAddRemoveResult
            {
                Date = date,
                Action = action,
                Reason = reason
            };
        }

        /// <summary>
        /// Adds selected dates taking into account excluded dates etc
        /// </summary>
        /// <param name="datesToBeAdded"></param>
        /// <returns></returns>
        public List<Models.AvailableDateAddRemoveResult> AddSelectedDates(List<Models.AvailableDates> datesToBeAdded)
        {
            // Creates a list of results to keep track of the added / skipped dates
            List<Models.AvailableDateAddRemoveResult> results = new List<Models.AvailableDateAddRemoveResult>();

            foreach (var d in datesToBeAdded)
            {
                // Check if the date is okay to be added to the list
                var reason = ValidateDate(d.Date);

                // Check if there was reason not to add it
                if (reason != null)
                {
                    // The date couldn't be added to the list of available dates
                    // Add the reason to the results and continue to the next date
                    results.Add(CreateResult(d.Date, "Skipped", $"{d.Date:dd/MM/yyyy} {reason}"));
                    continue;
                }

                // Validation passed and the date can be added to the list of available dates
                this.AvailableDates.Add(new Models.AvailableDates
                {
                    Date = d.Date,
                    IsPrimaryMatchday = (d.Date.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay) ? true : false,
                    IsBankHoliday = false,
                    Description = null
                });

                // Add the date being added to the results
                results.Add(CreateResult(d.Date, "Added", $"{d.Date:dd/MM/yyyy} was added"));
            }

            return results;
        }

        /// <summary>
        /// Tries to add all primary match days to the list of available dates using the settings.
        /// Takes into account any excluded dates, dates already in the list of available dates etc
        /// </summary>
        /// <returns></returns>
        public List<Models.AvailableDateAddRemoveResult> AddAllPrimaryMatchDaysDates()
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

                // Check if the date is okay to be added to the list
                var reason = ValidateDate(currentDate);

                // Check if there was reason not to add it
                if (reason != null)
                {
                    // The date couldn't be added to the list of available dates
                    // Add the reason to the results and continue to the next date
                    results.Add(CreateResult(currentDate, "Skipped", $"{currentDate.Date:dd/MM/yyyy} {reason}"));

                    // Add a day to the current date
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                // Validation passed and the date can be added to the list of available dates
                this.AvailableDates.Add(new Models.AvailableDates
                {
                    Date = currentDate.Date,
                    IsPrimaryMatchday = (currentDate.Date.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay) ? true : false,
                    IsBankHoliday = false,
                    Description = null
                });

                // Add the date being added to the results
                results.Add(CreateResult(currentDate.Date, "Added", $"{currentDate.Date:dd/MM/yyyy} was added"));

                //Add a day to the current date
                currentDate = currentDate.AddDays(1);
            }

            // Returns the added dates
            return results;
        }

        /// <summary>
        /// Allows the user to add selected bank holidays, taking into account 
        /// any excluded dates from the settings
        /// </summary>
        public List<Models.AvailableDateAddRemoveResult> AddBankHolidayDates(List<Models.BankHolidayEvent> bankHolidaysToBeAdded)
        {
            // Creates a list of results keep track of the added / skipped dates
            List<Models.AvailableDateAddRemoveResult> results = new List<Models.AvailableDateAddRemoveResult>();

            // Adds the bank holiday to the list of available days
            foreach (var bankHoliday in bankHolidaysToBeAdded)
            {
                // Check if the date is okay to be added to the list
                var reason = ValidateDate(bankHoliday.Date);

                if (reason != null)
                {
                    results.Add(CreateResult(bankHoliday.Date, "Skipped", $"{bankHoliday.Date:dd/MM/yyyy} ({bankHoliday.Title}) {reason}"));
                    continue;
                }

                // Validation passed and the date can be added to the list of available dates
                this.AvailableDates.Add(new Models.AvailableDates
                {
                    Date = bankHoliday.Date,
                    IsPrimaryMatchday = (bankHoliday.Date.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay) ? true : false,
                    IsBankHoliday = true,
                    Description = bankHoliday.Title
                });

                // Add the date being added to the results
                results.Add(CreateResult(bankHoliday.Date, "Added", $"{bankHoliday.Date:dd/MM/yyyy} ({bankHoliday.Title}) was added"));
            }

            return results;
        }

        /// <summary>
        /// Creates a list of primary matchdays between the dates in the application settings
        /// </summary>
        /// <returns></returns>
        private List<Models.AvailableDates> GetPrimaryDates()
        {
            var primaryDates = new List<AvailableDates>();

            // Find the first matching primary matchday on or after the startDate
            int daysUntilMatchday = ((int)_applicationSettings.Settings.PrimaryMatchDay - (int)_applicationSettings.Settings.StartDate.DayOfWeek + 7) % 7;
            var firstMatchDate = _applicationSettings.Settings.StartDate.AddDays(daysUntilMatchday);

            // Iterate by weeks instead of days
            for (var date = firstMatchDate; date <= _applicationSettings.Settings.EndDate; date = date.AddDays(7))
            {
                primaryDates.Add(new Models.AvailableDates
                {
                    Date = date,
                    IsPrimaryMatchday = date.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay
                });
            }

            return primaryDates.ToList();
        }

        /// <summary>
        /// Creates a list of alternative matchdays between the dates in the application settings
        /// </summary>
        /// <returns></returns>
        private List<Models.AvailableDates> GetAlternativeDates()
        {
            var alternativeDates = new List<AvailableDates>();

            // Find the first matching alternative matchday on or after the startDate
            int daysUntilMatchday = ((int)_applicationSettings.Settings.AlternativeMatchday - (int)_applicationSettings.Settings.StartDate.DayOfWeek + 7) % 7;
            var firstMatchDate = _applicationSettings.Settings.StartDate.AddDays(daysUntilMatchday);

            // Iterate by weeks instead of days
            for (var date = firstMatchDate; date <= _applicationSettings.Settings.EndDate; date = date.AddDays(7))
            {
                alternativeDates.Add(new Models.AvailableDates
                {
                    Date = date,
                    IsPrimaryMatchday = date.DayOfWeek == _applicationSettings.Settings.PrimaryMatchDay
                });
            }

            return alternativeDates.ToList();
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