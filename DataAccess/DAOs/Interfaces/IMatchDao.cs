
using Models.DTOs.Matches;
using Models.Enums;

namespace DataAccess.DAOs.Interfaces
{
    public interface IMatchDao
    {
        Task CreateMatchesAsync(List<MatchCreateRequestDto> matches);
        Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId);
        Task<MatchResponseDto> GetMatchByIdAsync(int matchId);
        Task<bool> UpdateMatchResultAsync(MatchResultRequestDto dto);
        Task<List<int>> GetWinnersByRoundAsync(int tournamentId, int round);
        Task<int> GetCurrentMaxRoundAsync(int tournamentId);
    }
}
