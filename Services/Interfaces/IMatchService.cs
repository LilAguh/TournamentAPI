
using Models.DTOs.Matches;

namespace Services.Interfaces
{
    public interface IMatchService
    {
        Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds);
        Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId);
    }
}
