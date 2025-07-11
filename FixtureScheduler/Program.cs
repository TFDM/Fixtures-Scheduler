using Microsoft.Extensions.DependencyInjection;

//Loads the settings
var settings = BusinessLogic.ApplicationSettings.LoadSettings();

if (settings == null)
{
    return;
}

//Defines the number of teams
var totalNumberOfTeams = 24;

//Sets the number of rounds needed to play all the games
//This is (Number of Teams x 2 - 2)
var numberOfRoundsNeeded = (totalNumberOfTeams - 1) * 2;

//Sets up the dependency injection and registers interfaces and their implementation classes
//Also passes in the settings created earlier so these can be used elsewhere in the application
var serviceProvider = new ServiceCollection()
    .AddSingleton(settings)
    .AddSingleton<Interfaces.IBankHolidays, BusinessLogic.BankHolidays>()
    .AddSingleton<Interfaces.IDates, BusinessLogic.Dates>()
    .BuildServiceProvider();

//Creates an instance of dates
var dates = serviceProvider.GetRequiredService<Interfaces.IDates>();

var availableDates = dates.GetAvailableDates().OrderBy(x => x.Date);

//Counts the number of primary matchdays
var numberOfPrimaryMatchdays = dates.CountMatchdays(useAlternativeMatchday: false);

if (numberOfPrimaryMatchdays < numberOfRoundsNeeded)
{
    Console.WriteLine("Not enough primary matchdays");
}

//Counts the number of saturdays between the two dates
int count = dates.CountMatchdays(useAlternativeMatchday: false);

//This will get removed at some point
Console.WriteLine("Hello, World!");
Console.WriteLine(count.ToString());

var y = Console.ReadLine();

Console.WriteLine(y);
Console.Read();