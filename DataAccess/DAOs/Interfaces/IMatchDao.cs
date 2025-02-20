
namespace DataAccess.DAOs.Interfaces
{
    public interface IMatchDao
    {
        Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds);
    }
}
