
using Models.DTOs.Matches;

namespace DataAccess.DAOs.Interfaces
{
    public interface IMatchDao
    {
        Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds, int round);
        Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId);
        Task<MatchResponseDto?> GetMatchByIdAsync(int matchId);
        Task<bool> UpdateMatchWinnerAsync(int matchId, int winnerId);
        Task<List<int>> GetWinnersByRoundAsync(int tournamentId, int round);
        Task<bool> IsFirstRoundAsync(int tournamentId);
        Task<int> GetLastRoundAsync(int tournamentId);
    }
}
