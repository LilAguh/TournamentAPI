
using Models.DTOs.Tournament;

namespace Services.Interfaces
{
    public interface ITournamentService
    {
        Task<TournamentResponseDto> CreateTournamentAsync(TournamentRequestDto dto, int organizerId);
        Task<TournamentResponseDto> GetTournamentByIdAsync(int tournamentId);
        Task<bool> RegisterPlayerAsync(int tournamentId, int userId, int deckId);
        Task AddAllowedSeriesAsync(int tournamentId, int seriesId);
        Task RemoveAllowedSeriesAsync(int tournamentId, int seriesId);
        Task<List<int>> GetAllowedSeriesAsync(int tournamentId);
    }
}
