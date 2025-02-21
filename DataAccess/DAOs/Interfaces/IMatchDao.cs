
using Models.DTOs.Matches;

namespace DataAccess.DAOs.Interfaces
{
    public interface IMatchDao
    {
        Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds);
        Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId);
    }
}
