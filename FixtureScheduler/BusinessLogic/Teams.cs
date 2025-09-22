using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace BusinessLogic
{
    public class Teams : Interfaces.ITeams
    {
        private readonly Interfaces.IUserInterface _userInterface;
        public List<Models.Teams> ListOfTeams { get; private set; } = new List<Models.Teams>();

        public Teams(Interfaces.IUserInterface userInterface)
        {
            // User interface passed in by via dependency injection
            _userInterface = userInterface;

            // Attempt to load the teams from the Teams.json file
            // They will be checked to ensure they are valid
            ListOfTeams = LoadTeams();
        }

        /// <summary>
        /// Loads teams from the Settings/Teams.json file
        /// </summary>
        /// <returns></returns>
        private List<Models.Teams> LoadTeams()
        {
            try
            {
                var exePath = AppContext.BaseDirectory;

                // Loads the teams json file
                var config = new ConfigurationBuilder()
                    .SetBasePath(exePath)
                    .AddJsonFile("Settings/Teams.json", optional: false, reloadOnChange: false)
                    .Build();

                // Bind the json to the teams dto
                List<DTOs.TeamsDTO> teamsDTO = config.GetSection("Teams")
                    .Get<List<DTOs.TeamsDTO>>()!;

                // Validates the teams using fluent validation
                var valid = IsValid(teamsDTO);

                if (valid)
                {
                    // Convert to a list of teams model and return
                    return teamsDTO.Select(dto => new Models.Teams
                    {
                        Name = dto.Name
                    }).ToList();
                }

                // Throw an exception if the teams file failed validation checks
                throw new FixtureSchedulerException("The Teams.json file wasn't valid. Please check the errors.");
            }
            catch (FileNotFoundException)
            {
                // Throw an exception if the file can't be found
                throw new FixtureSchedulerException("Unable to find the Teams.json file in the Settings folder.");
            }
        }

        /// <summary>
        /// Validates the teams
        /// </summary>
        /// <param name="teamsDTO"></param>
        /// <returns></returns>
        private static bool IsValid(List<DTOs.TeamsDTO> teamsDTO)
        {
            var allValid = true;

            // Creates the fluent validator
            var validator = new Validation.TeamsValidation();

            // Loops over team in the dto
            foreach (var team in teamsDTO)
            {
                // Validates the team
                var validationResults = validator.Validate(team);

                // If the validation fails then the error results are returned to the user
                if (!validationResults.IsValid)
                {
                    // Validation has failed - set the allValid variable to false
                    // and display each of the error messages from the validator
                    allValid = false;
                    foreach (var error in validationResults.Errors)
                    {
                        Console.WriteLine(error.PropertyName + ":" + error.ErrorMessage);
                    }
                }
            }

            // Returns true or false based on the validation
            return allValid;
        }

        /// <summary>
        /// Shows the teams to the user in a table
        /// </summary>
        public void ShowTeams()
        {
            // Creates a counter - this is used to create a column 
            // with a number in or the table
            var count = 1;
            var teams = new List<List<string>>();

            // Loop over the teams and for each one create a new list of string
            // This list is comprised of the count and the team name
            foreach (var team in this.ListOfTeams)
            {
                teams.Add(new List<string> { $"{count}", $"{team.Name}" });
                count++;
            }

            // Create the table
            _userInterface.ShowTable(
                headers: new List<string>
                    {"#", "Team"},
                rows: teams
            );
        }
    }
}