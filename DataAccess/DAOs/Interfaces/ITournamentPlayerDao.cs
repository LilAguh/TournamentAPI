
namespace DataAccess.DAOs.Interfaces
{
    public interface ITournamentPlayerDao
    {
        Task<bool> RegisterPlayerAsync(int tournamentId, int userId, int deckId);
        Task<bool> IsPlayerRegisteredAsync(int tournamentId, int userId);
        Task<List<int>> GetPlayerIdsAsync(int tournamentId);
    }
}
