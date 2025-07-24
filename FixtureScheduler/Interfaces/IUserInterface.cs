using Spectre.Console;

namespace Interfaces
{
    public interface IUserInterface
    {
        void ShowMarkUpMessage(Markup message, bool addWriteLine = false);
        List<T> AskMultiSelection<T>(string prompt, List<T> options) where T : notnull;
        void ShowTable(List<string> headers, List<List<string>> rows, string? title = null);
    }
}