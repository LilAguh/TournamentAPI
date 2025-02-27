
using Models.DTOs.Tournament;

namespace DataAccess.DAOs.Interfaces
{
    public interface ITournamentDao
    {
        Task<int> AddTournamentAsync(TournamentRequestDto dto, int organizerId);
        Task<TournamentResponseDto> GetTournamentByIdAsync(int tournamentID);
        Task<IEnumerable<TournamentResponseDto>> GetAllTournamentsAsync();
        Task<TournamentResponseDto> GetTournamentByPhaseAsync(int tournamentPhase);
        Task<int> CalculateMaxPlayersAsync(TournamentRequestDto dto);
        Task UpdateTournamentPhaseAsync(int tournamentId, string newPhase);
        Task IncrementCountPlayersAsync(int tournamentId);
        Task FinalizeTournamentAsync(int tournamentId, int winnerId);
        Task<List<int>> GetEnabledSeriesAsync(int tournamentId);
        Task AddAllowedSeriesAsync(int tournamentId, int seriesId);
        Task RemoveAllowedSeriesAsync(int tournamentId, int seriesId);
        Task<List<int>> GetAllowedSeriesAsync(int tournamentId);
    }
}
