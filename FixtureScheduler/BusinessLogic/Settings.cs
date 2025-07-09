using Microsoft.Extensions.Configuration;

namespace BusinessLogic
{
    public class Settings
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
                //Loads the settings configuration file
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Settings/Settings.json", optional: false, reloadOnChange: false)
                    .Build();

                //Bind the settings to the settings model object
                Models.Settings settings = config.GetSection("Settings").Get<Models.Settings>()!;

                //Returns the settings
                return settings;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Unable to find the Settings.json file in the Settings folder");
                Console.WriteLine("Unable to proccede without settings file");
            }

            return null;
        }
    }
}