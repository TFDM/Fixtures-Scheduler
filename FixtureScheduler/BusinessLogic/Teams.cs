using Microsoft.Extensions.Configuration;

namespace BusinessLogic
{
    public class Teams
    {
        /// <summary>
        /// Loads teams from the Settings/Teams.json file
        /// </summary>
        /// <returns></returns>
        public static List<Models.Teams> LoadTeams()
        {
            try
            {
                var exePath = AppContext.BaseDirectory;

                //Loads the teams json file
                var config = new ConfigurationBuilder()
                    .SetBasePath(exePath)
                    .AddJsonFile("Settings/Teams.json", optional: false, reloadOnChange: false)
                    .Build();

                //Bind the json to the teams dto
                List<DTOs.TeamsDTO> teamsDTO = config.GetSection("Teams")
                    .Get<List<DTOs.TeamsDTO>>()!;

                //Validates the teams using fluent validation
                var valid = IsValid(teamsDTO);

                if (valid)
                {
                    //Convert to a list of teams model and return
                    return teamsDTO.Select(dto => new Models.Teams
                    {
                        Name = dto.Name
                    }).ToList();
                }

                //Throw an exception if the teams file failed validation checks
                throw new FixtureSchedulerException("The Teams.json file wasn't valid. Please check the errors.");
            }
            catch (FileNotFoundException)
            {
                //Throw an exception if the file can't be found
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

            //Creates the fluent validator
            var validator = new Validation.TeamsValidation();

            //Loops over team in the dto
            foreach (var team in teamsDTO)
            {
                //Validates the team
                var validationResults = validator.Validate(team);

                //if the validation fails then the error results are returned to the user
                if (!validationResults.IsValid)
                {
                    //Validation has failed - set the allValid variable to false
                    //and display each of the error messages from the validator
                    allValid = false;
                    foreach (var error in validationResults.Errors)
                    {
                        Console.WriteLine(error.PropertyName + ":" + error.ErrorMessage);
                    }
                }
            }

            //Returns true or false based on the validation
            return allValid;
        }
    }
}