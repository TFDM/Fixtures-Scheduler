using BusinessLogic;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

// Sets up the dependency injection and registers interfaces and their implementation classes
var serviceProvider = new ServiceCollection()
    .AddSingleton<Interfaces.ITeams, BusinessLogic.Teams>()
    .AddSingleton<Interfaces.IApplicationSettings, BusinessLogic.ApplicationSettings>()
    .AddSingleton<Interfaces.IBankHolidays, BusinessLogic.BankHolidays>()
    .AddSingleton<Interfaces.IDates, BusinessLogic.Dates>()
    .BuildServiceProvider();

try
{
    // Resolve IApplicationSettings and teams
    var applicationSettings = serviceProvider.GetRequiredService<Interfaces.IApplicationSettings>();
    var teams = serviceProvider.GetRequiredService<Interfaces.ITeams>();

    // Display the teams in a table
    teams.ShowTeams();

    // Ask the user to confirm
    //  var confirmation = AnsiConsole.Prompt(
    //      new TextPrompt<bool>("Are the teams correct?")
    //          .AddChoice(true)
    //          .AddChoice(false)
    //          .DefaultValue(true)
    //          .WithConverter(choice => choice ? "y" : "n"));

    // AnsiConsole.MarkupLine(confirmation ? "[green]Yes[/]" : "[red]No[/]");

    // Display the settings in a table
    applicationSettings.ShowSettings();

    // Creates an instance of dates - this will generate a list of initial 
    // dates for the primary matchday only
    var dates = serviceProvider.GetRequiredService<Interfaces.IDates>();

    AnsiConsole.Write(new Markup($"Number of initial match days found: {dates.AvailableDates.Count()}"));
    AnsiConsole.WriteLine();

    // Checks if more dates are required
    if (dates.MoreDatesRequired())
    {
        AnsiConsole.Write(new Markup($"[red]Not enough days for the number of rounds required[/]"));
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup($"{dates.TotalNumberOfAdditionalDatesRequired()} more dates are required"));
        AnsiConsole.WriteLine();

        // Ask the user if they would like to add bank holidays confirm
        var confirmation = AnsiConsole.Prompt(
            new TextPrompt<bool>("Would you like to add bank holidays to the list of available dates?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup (confirmation ? "[green]Adding bank holidays...[/]" : "[red]No bank holidays added[/]"));
        AnsiConsole.WriteLine();

        // Add the bank holidays
        dates.AddBankHolidayDates();
    }

    AnsiConsole.Write(new Markup($"Number of match days found: {dates.AvailableDates.Count()}"));
    AnsiConsole.WriteLine();

    // Checks if more dates are required
    if (dates.MoreDatesRequired())
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup($"[red]Not enough days for the number of rounds required[/]"));
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup($"{dates.TotalNumberOfAdditionalDatesRequired()} more dates are required"));
    }

}
catch (FixtureSchedulerException ex)
{
    Console.WriteLine(ex.Message);
    Environment.Exit(1);
}