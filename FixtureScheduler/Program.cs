using BusinessLogic;
using Microsoft.Extensions.DependencyInjection;

List<Models.Teams> teams;

try
{
    //Loads the teams from the teams json files
    teams = BusinessLogic.Teams.LoadTeams();
}
catch (FixtureSchedulerException ex)
{
    //Exits if the 
    Console.WriteLine(ex.Message);
    Environment.Exit(1);

    //This return will never be reached but seems to be required to keep the compiler happy :)
    return;
}

//Sets up the dependency injection and registers interfaces and their implementation classes
//Also passes in the teams created earlier so these can be used elsewhere in the application
var serviceProvider = new ServiceCollection()
    .AddSingleton(teams)
    .AddSingleton<Interfaces.IApplicationSettings, BusinessLogic.ApplicationSettings>()
    .AddSingleton<Interfaces.IBankHolidays, BusinessLogic.BankHolidays>()
    .AddSingleton<Interfaces.IDates, BusinessLogic.Dates>()
    .BuildServiceProvider();

//Creates an instance of dates
var dates = serviceProvider.GetRequiredService<Interfaces.IDates>();

var availableDates = dates.GetAvailableDates().OrderBy(x => x.Date);

//Counts the number of primary matchdays
//var numberOfPrimaryMatchdays = dates.CountMatchdays(useAlternativeMatchday: false);

//if (numberOfPrimaryMatchdays < numberOfRoundsNeeded)
//{
//    Console.WriteLine("Not enough primary matchdays");
//}

//Counts the number of saturdays between the two dates
int count = dates.CountMatchdays(useAlternativeMatchday: false);

//This will get removed at some point
Console.WriteLine("Hello, World!");
Console.WriteLine(count.ToString());

var y = Console.ReadLine();

Console.WriteLine(y);
Console.Read();