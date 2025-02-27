
using Models.DTOs.Disqualifications;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IDisqualificationDao
    {
        Task<int> AddDisqualificationAsync(Disqualification disqualification);
        Task<IEnumerable<DisqualificationResponseDto>> GetDisqualificationsByTournamentAsync(int tournamentId);
    }
}
