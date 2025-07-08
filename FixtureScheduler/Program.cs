using Microsoft.Extensions.DependencyInjection;

//Sets up the dependency injection and registers interfaces and their implementation classes
var serviceProvider = new ServiceCollection()
    .AddSingleton<Interfaces.IDatesFactory, BusinessLogic.DatesFactory>()
    .BuildServiceProvider();

//Creates an instance of the DatesFactory
var datesFactory = serviceProvider.GetRequiredService<Interfaces.IDatesFactory>();

//Define a start date and an end date
//Also sets a primary matchday and an alternative matchday
var start = new DateTime(2025, 8, 2);
var end = new DateTime(2026, 5, 1);
var primaryMatchday = DayOfWeek.Saturday;
var altMatchDay = DayOfWeek.Tuesday;
var totalNumberOfTeams = 24;



//Sets the number of rounds needed to play all the games
//This is (Number of Teams x 2 - 2)
var numberOfRoundsNeeded = (totalNumberOfTeams - 1) * 2;

//Uses the DatesFactory to create an instance of Dates
//The start and end dates specified above are passed into the create method
var dates = datesFactory.Create(start, end, primaryMatchday, altMatchDay);



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