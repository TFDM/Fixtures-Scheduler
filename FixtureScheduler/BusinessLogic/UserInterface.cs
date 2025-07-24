using Spectre.Console;

namespace BusinessLogic
{
    public class UserInterface : Interfaces.IUserInterface
    {
        /// <summary>
        /// Shows a spectre console mark up message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="addWriteLine"></param>
        public void ShowMarkUpMessage(Markup message, bool addWriteLine = false)
        {
            AnsiConsole.Write(message);
            if (addWriteLine)
                AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Shows a spectre console multi selection prompt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prompt"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public List<T> AskMultiSelection<T>(string prompt, List<T> options) where T : notnull
        {
            // Where T added to keep the compiler happy - T will never be null
            // This 
            return AnsiConsole.Prompt(
                new MultiSelectionPrompt<T>()
                    .Title(prompt)
                    .Required()
                    .InstructionsText("Use the spacebar to select each option and then hit return when done")
                    .AddChoices(options)
            );
        }

        /// <summary>
        /// Shows a spectre console table
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="rows"></param>
        /// <param name="title"></param>
        public void ShowTable(List<string> headers, List<List<string>> rows, string? title = null)
        {
            // Create a table
            var table = new Table();
            table.Border = TableBorder.Horizontal;

            if (title != null)
                table.Caption(title);

            // Add the columms by looping over the headers
            foreach (var header in headers)
            {
                table.AddColumn(new TableColumn(header));
            }

            // Add the rows
            foreach (var row in rows)
            {
                // For each row convert its list of string to an array
                table.AddRow(row.ToArray());
            }

            // Render the table back to the console
            AnsiConsole.Write(table);
        }
    }
}