using System.Globalization;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace BusinessLogic
{
    public class ApplicationSettings : Interfaces.IApplicationSettings
    {
        private readonly Interfaces.ITeams _teams;
        public Models.Settings Settings { get; private set; }

        public ApplicationSettings(Interfaces.ITeams teams)
        {
            // Teams received via dependency injection
            _teams = teams;

            // Attempt to load the settings from the Settings.json file
            // They will also be checked to see if they are valid
            Settings = LoadSettings();

            // The number of rounds needed is calculated using the teams
            Settings.NumberOfRoundsNeeded = (_teams.ListOfTeams.Count() - 1) * 2;
        }

        /// <summary>
        /// Loads the settings from the Settings/Settings.json file
        /// </summary>
        /// <returns></returns>
        private Models.Settings LoadSettings()
        {
            try
            {
                var exePath = AppContext.BaseDirectory;

                // Loads the settings configuration file
                var config = new ConfigurationBuilder()
                    .SetBasePath(exePath)
                    .AddJsonFile("Settings/Settings.json", optional: false, reloadOnChange: false)
                    .Build();

                // Bind the json to the application settings dto. This is done so that all the 
                // values in the json file can be strings to avoid any crashes if the user enters 
                // bad values - this will be checked anyway with validation
                DTOs.ApplicationSettingsDTO settingsDTO = config.GetSection("Settings")
                    .Get<DTOs.ApplicationSettingsDTO>()!;

                // Checks the settings are valid using fluent validation
                var valid = IsValid(settingsDTO);

                if (valid)
                {
                    // Convert to a Settings model and return
                    return new Models.Settings()
                    {
                        StartDate = DateTime.ParseExact(settingsDTO.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        EndDate = DateTime.ParseExact(settingsDTO.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        PrimaryMatchDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), settingsDTO.PrimaryMatchday, true),
                        AlternativeMatchday = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), settingsDTO.AlternativeMatchday, true),
                        ExcludedDates = settingsDTO.ExcludedDates
                            .Select(dto => new Models.ExcludedDates
                            {
                                Date = DateTime.ParseExact(dto.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                Reason = dto.Reason
                            }).ToList()
                    };
                }

                // Throw an exception
                throw new FixtureSchedulerException("The Settings.json file wasn't valid. Please check the errors.");
            }
            catch (FileNotFoundException)
            {
                // Throw an exception in the event the file can't be found
                throw new FixtureSchedulerException(@"Unable to find the Settings.json file in the settings folder.");
            }
        }

        /// <summary>
        /// Validates values in the json file
        /// </summary>
        /// <param name="settingsDTO"></param>
        /// <returns></returns>
        private static bool IsValid(DTOs.ApplicationSettingsDTO settingsDTO)
        {
            // Creates the fluent validator
            var validator = new Validation.ApplicationSettingsValidation();

            // Validates the dto
            var validationResults = validator.Validate(settingsDTO);

            // If the validation is not valid then each of the errors are displayed
            if (!validationResults.IsValid)
            {
                Console.WriteLine("There were problems!");
                foreach (var error in validationResults.Errors)
                {
                    Console.WriteLine(error.PropertyName + ":" + error.ErrorMessage);
                }

                // Not valid so returns false
                return false;
            }

            // No validation issues found so returns true
            return true;
        }

        /// <summary>
        /// Shows the settings to the user in a table
        /// </summary>
        public void ShowSettings()
        {
            AnsiConsole.WriteLine("Settings loaded from Settings.json file");

            // Check if the settings have any excluded dates
            var excludedDatesAsString = this.Settings.ExcludedDates.Any()
                // If they do, join all the dates together into a single string with a \n seperating each one
                ? string.Join("\n", this.Settings.ExcludedDates.Select(d =>
                    $"{d.Date:dd/MM/yyyy} - ({d.Reason})"))
                // If there are no dates return this string
                : "No excluded dates";

            // Create a table
            var table = new Table();
            table.Border = TableBorder.Horizontal;

            // Add the columns - Adding .LeftAligned afterwards ensures just the colum
            // headers are centred and not the content
            table.AddColumn(new TableColumn("Setting"));
            table.AddColumn(new TableColumn("Value"));

            // Add rows to show the settings
            table.AddRow("Start Date", $"{this.Settings.StartDate:dd/MM/yyyy}");
            table.AddRow("End Date", $"{this.Settings.EndDate:dd/MM/yyyy}");
            table.AddRow(new Markup("Excluded Dates"), new Markup(excludedDatesAsString));
            table.AddRow("Primary Match Day", $"{this.Settings.PrimaryMatchDay}");
            table.AddRow("Alternative Match Day", $"{this.Settings.AlternativeMatchday}");
            table.AddRow("Number of Rounds Needed", $"{this.Settings.NumberOfRoundsNeeded}");

            // Render the table to the console
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
    }
}