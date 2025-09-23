using System.Collections;
using BusinessLogic;
using Spectre.Console;
using Interfaces;

public class DatesManager : Interfaces.IDatesManager
{
    private readonly IDates _dates;
    private readonly IApplicationSettings _applicationSettings;

    public DatesManager(IDates dates, IApplicationSettings applicationSettings)
    {
        // Dates interface is passed in via dependency injection
        _dates = dates;

        // Application settings interface is passed in via dependency injection
        _applicationSettings = applicationSettings;
    }

    /// <summary>
    /// The main entry point for the dates manager
    /// </summary>
    public void Run()
    {
        AnsiConsole.Clear();

        // Create a new list of menu items
        List<Models.MenuItem> mainMenuOptions = new List<Models.MenuItem>();
        mainMenuOptions.Add(new Models.MenuItem { Option = 1, Description = "Display Exisiting Available Dates" });
        mainMenuOptions.Add(new Models.MenuItem { Option = 2, Description = "Add Primary Match Days in Bulk" });
        mainMenuOptions.Add(new Models.MenuItem { Option = 3, Description = "Add Selected Primary Match Days" });
        mainMenuOptions.Add(new Models.MenuItem { Option = 4, Description = "Add Selected Alternative Match Days" });
        mainMenuOptions.Add(new Models.MenuItem { Option = 5, Description = "Add Bank Holidays" });
        mainMenuOptions.Add(new Models.MenuItem { Option = 6, Description = "Exit" });

        // Sets an exit variable so the dates manager menu will
        // continue to be displayed until the user picks the exit option
        bool exit = false;

        while (!exit)
        {
            AnsiConsole.Clear();

            // Displays the title for the dates manager
            Panel title = new Panel(new Markup("[bold yellow]Dates Manager[/]"));
            AnsiConsole.Write(title);

            // Shows the number of available dates
            ShowNumberOfAvailableDates();

            // Ask the user to select a menu option
            Models.MenuItem selectedMenuOption = AnsiConsole.Prompt(
                new SelectionPrompt<Models.MenuItem>()
                .Title("Please select:")
                .AddChoices(mainMenuOptions)
            );

            // Loads the appropriate function based on the user's choice
            switch (selectedMenuOption.Option)
            {
                case 1:
                    ShowAvailableDates();
                    break;
                case 2:
                    AddPrimaryMatchDays(inBulk: true);
                    break;
                case 3:
                    AddPrimaryMatchDays();
                    break;
                case 4:
                    AddAlternativeMatchDays();
                    break;
                case 5:
                    AddBankHolidays();
                    break;
                case 6:
                    // User picked the exit option
                    // Set the exit variable back to 
                    // true to break out of the while loop
                    exit = true;
                    break;
            }
        }
    }

    /// <summary>
    /// Shows the number of available dates and a message if more dates are required
    /// </summary>
    private void ShowNumberOfAvailableDates()
    {
        int count = _dates.AvailableDates.Count();

        // Displays the number of available dates
        AnsiConsole.Write(
            new Markup($"Number of available dates: {(count > 0 ? $"[green]{count}[/]" : $"[red]{count}[/]")}")
        );

        // Displays a message if more dates are required or if there are sufficient dates
        AnsiConsole.Write(
            new Markup(_dates.MoreDatesRequired() ?
                " [red](Not enough dates for the number of rounds required)[/]" :
                " [green](Sufficient dates for the number of rounds required)[/]")
        );


        AnsiConsole.WriteLine($"\nNumber of additional dates required: {_dates.TotalNumberOfAdditionalDatesRequired()}");

        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Shows the available dates that have been added
    /// </summary>
    private void ShowAvailableDates()
    {
        AnsiConsole.Clear();
        
        int count = _dates.AvailableDates.Count();

        // Shows the number of available dates
        ShowNumberOfAvailableDates();

        if (_dates.AvailableDates.Count() > 0)
        {
            AnsiConsole.WriteLine();

            // Creates a new table for the results
            var table = new Table();
            table.Border = TableBorder.Horizontal;
            table.AddColumn("Date");
            table.AddColumn("Is Primary Matchday?");
            table.AddColumn("Is Bank Holiday?");

            AnsiConsole.Live(table).Start(ctx =>
            {
                foreach (var date in _dates.AvailableDates.OrderBy(d => d.Date))
                {
                    table.AddRow(
                        $"{date.Date:dd/MM/yyyy}", 
                        (date.IsPrimaryMatchday) ? "[green]Yes[/]" : "[red]No[/]",
                        (date.IsBankHoliday) ? "[green]Yes[/]" : "[red]No[/]"
                    );

                    ctx.Refresh();
                    Thread.Sleep(500);
                }
            });
        }
        else
        {
            // No available dates
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Markup("[red]There are currently no available dates.[/]"));
            AnsiConsole.WriteLine();
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup("Press any key to continue"));
        Console.ReadKey();
    }

    /// <summary>
    /// Adds primary match days to the list of available dates
    /// </summary>
    private void AddPrimaryMatchDays(bool inBulk = false)
    {
        List<Models.AvailableDateAddRemoveResult> results;

        if (inBulk)
        {
            // Add the primary match days in bulk
            results = _dates.AddAllPrimaryMatchDaysDates();
        }
        else
        {
            // Primary match days aren't being added in bulk. The user will need to select them

            // Shows a message to the user to explain the dates showen are
            // betwen the start date and end date set in the Settings.json file
            AnsiConsole.Write(new Markup(
                $"Showing {_applicationSettings.Settings.PrimaryMatchDay}'s between " +
                $"{_applicationSettings.Settings.StartDate:dd/MM/yyyy} and " +
                $"{_applicationSettings.Settings.EndDate:dd/MM/yyyy}"
            ));

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            // Ask the user to select primary match days
            List<Models.AvailableDates> selectedDays = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Models.AvailableDates>()
                .Title("Please select from the primary match days shown below:")
                .AddChoices(_dates.PrimaryDates!)
            );

            AnsiConsole.Clear();

            // Add the selected dates
            results = _dates.AddSelectedDates(selectedDays);
        }

        // Creates a new table for the results
        var table = new Table();
        table.Border = TableBorder.Horizontal;
        table.AddColumn("Date");
        table.AddColumn("Action");
        table.AddColumn("Reason");

        AnsiConsole.Live(table).Start(ctx =>
        {
            // Loops over each of the results and creates a table row
            foreach (var r in results)
            {
                table.AddRow(
                    $"{r.Date:dd/MM/yyyy}",
                    (r.Action == "Skipped") ? $"[red]{r.Action}[/]" : $"[green]{r.Action}[/]",
                    $"{r.Reason}"
                );

                ctx.Refresh();
                Thread.Sleep(500);
            }
        });

        AnsiConsole.Write(new Markup("Press any key to continue"));
        Console.ReadKey();
    }

    /// <summary>
    /// Adds bank holidays to the list of available dates
    /// </summary>
    private void AddBankHolidays()
    {
        // Shows a message to the user to explain the bank holidays showen are
        // betwen the start date and end date set in the Settings.json file
        AnsiConsole.Write(new Markup(
            $"Showing bank holidays between " +
            $"{_applicationSettings.Settings.StartDate:dd/MM/yyyy} and " +
            $"{_applicationSettings.Settings.EndDate:dd/MM/yyyy}"
        ));

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();

        // Ask the user to select bank holidays
        List<Models.BankHolidayEvent> selectedBankHolidays = AnsiConsole.Prompt(
            new MultiSelectionPrompt<Models.BankHolidayEvent>()
            .Title("Please select from the bank holidays shown below:")
            .AddChoices(_dates.BankHolidays!)
        );

        AnsiConsole.Clear();

        // Adds the selected bank holiday dates and return a set of results to show what was added / skipped
        var results = _dates.AddBankHolidayDates(selectedBankHolidays);

        // Creates a new table for the results
        var table = new Table();
        table.Border = TableBorder.Horizontal;
        table.AddColumn("Date");
        table.AddColumn("Action");
        table.AddColumn("Reason");

        AnsiConsole.Live(table).Start(ctx =>
        {
            // Loops over each of the results and creates a table row
            foreach (var r in results)
            {
                table.AddRow(
                    $"{r.Date:dd/MM/yyyy}",
                    (r.Action == "Skipped") ? $"[red]{r.Action}[/]" : $"[green]{r.Action}[/]",
                    $"{r.Reason}"
                );

                ctx.Refresh();
                Thread.Sleep(500);
            }
        });

        AnsiConsole.Write(new Markup("Press any key to continue"));
        Console.ReadKey();
    }

    /// <summary>
    /// Adds alternative match days to the list of available dates
    /// </summary>
    private void AddAlternativeMatchDays()
    {
        // Shows a message to the user to explain the dates showen are
        // betwen the start date and end date set in the Settings.json file
        AnsiConsole.Write(new Markup(
            $"Showing {_applicationSettings.Settings.AlternativeMatchday}'s between " +
            $"{_applicationSettings.Settings.StartDate:dd/MM/yyyy} and " +
            $"{_applicationSettings.Settings.EndDate:dd/MM/yyyy}"
        ));

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();

        // Ask the user to select alternative match days
        List<Models.AvailableDates> selectedDays = AnsiConsole.Prompt(
            new MultiSelectionPrompt<Models.AvailableDates>()
            .Title("Please select from the alternative match days shown below:")
            .AddChoices(_dates.AlternativeDates!)
        );

        AnsiConsole.Clear();

        // Add the selected dates
        var results = _dates.AddSelectedDates(selectedDays);

        // Creates a new table for the results
        var table = new Table();
        table.Border = TableBorder.Horizontal;
        table.AddColumn("Date");
        table.AddColumn("Action");
        table.AddColumn("Reason");

        AnsiConsole.Live(table).Start(ctx =>
        {
            // Loops over each of the results and creates a table row
            foreach (var r in results)
            {
                table.AddRow(
                    $"{r.Date:dd/MM/yyyy}",
                    (r.Action == "Skipped") ? $"[red]{r.Action}[/]" : $"[green]{r.Action}[/]",
                    $"{r.Reason}"
                );

                ctx.Refresh();
                Thread.Sleep(500);
            }
        });

        AnsiConsole.Write(new Markup("Press any key to continue"));
        Console.ReadKey();

    }
}