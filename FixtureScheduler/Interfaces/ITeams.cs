namespace Interfaces
{
    public interface ITeams
    {
        List<Models.Teams> ListOfTeams { get; }

        void ShowTeams();
    }
}