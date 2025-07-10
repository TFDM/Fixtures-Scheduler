using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic
{
    public class ApplicationSettings
    {
        /// <summary>
        /// Loads the settings from the Settings/Settings.json file
        /// If the settings file can't be load it returns null
        /// </summary>
        /// <returns></returns>
        public static Models.Settings? LoadSettings()
        {
            try
            {
                var exePath = AppContext.BaseDirectory;

                //Loads the settings configuration file
                var config = new ConfigurationBuilder()
                    .SetBasePath(exePath)
                    .AddJsonFile("Settings/Settings.json", optional: false, reloadOnChange: false)
                    .Build();

                //Bind the json to the application settings dto. This is done so that all the 
                //values in the json file can be strings to avoid any crashes if the user enters 
                //bad values - this will be checked anyway with validation
                DTOs.ApplicationSettingsDTO settingsDTO = config.GetSection("Settings")
                    .Get<DTOs.ApplicationSettingsDTO>()!;

                //Checks the settings are valid using fluent validation
                var valid = IsValid(settingsDTO);

                if (valid)
                {
                    //Convert to a Settings model and return
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

                return null;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Unable to find the Settings.json file in the Settings folder");
                Console.WriteLine("Unable to proccede without settings file");
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Validates values in the json file
        /// </summary>
        /// <param name="settingsDTO"></param>
        /// <returns></returns>
        private static bool IsValid(DTOs.ApplicationSettingsDTO settingsDTO)
        {
            //Creates the fluent validator
            var validator = new Validation.ApplicationSettingsValidation();

            //Validates the dto
            var validationResults = validator.Validate(settingsDTO);

            //If the validation is not valid then each of the errors are displayed
            if (!validationResults.IsValid)
            {
                Console.WriteLine("There were problems!");
                foreach (var error in validationResults.Errors)
                {
                    Console.WriteLine(error.PropertyName + ":" + error.ErrorMessage);
                }

                //Not valid so returns false
                return false;
            }

            //No validation issues found so returns true
            return true;
        }
    }
}