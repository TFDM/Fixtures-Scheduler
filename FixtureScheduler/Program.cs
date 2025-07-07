using Microsoft.Extensions.DependencyInjection;

//Sets up the dependency injection and registers interfaces and their implementation classes
var serviceProvider = new ServiceCollection()
    .AddSingleton<Interfaces.IDatesFactory, BusinessLogic.DateFactory>()
    .BuildServiceProvider();

//Creates an instance of the DatesFactory
var datesFactory = serviceProvider.GetRequiredService<Interfaces.IDatesFactory>();

//Define a start date and an end date
var start = new DateTime(2025, 8, 2);
var end = new DateTime(2026, 5, 1);

//Uses the DatesFactory to create an instance of Dates
//The start and end dates specified above are passed into the create method
var dates = datesFactory.Create(start, end);

//Counts the number of saturdays between the two dates
int count = dates.CountSaturdays();

//This will get removed at some point
Console.WriteLine("Hello, World!");
Console.WriteLine(count.ToString());

var y = Console.ReadKey();
