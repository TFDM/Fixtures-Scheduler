namespace Interfaces
{
    public interface IApplicationSettings
    {
        Models.Settings Settings { get; }

        void ShowSettings();
    }
}