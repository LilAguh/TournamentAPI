
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Matches;
using Services.Interfaces;

namespace Services.Implementations
{
    public class MatchService : IMatchService
    {
        private readonly IMatchDao _matchDao;

        public MatchService(IMatchDao matchDao)
        {
            _matchDao = matchDao;
        }

        public async Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds)
        {
            // Aquí se podrían agregar validaciones adicionales (por ejemplo, verificar que playerIds.Count == tournament.MaxPlayers)
            await _matchDao.CreateRoundMatchAsync(tournamentId, playerIds);
        }

        public async Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId)
        {
            return await _matchDao.GetMatchesByTournamentAsync(tournamentId);
        }
    }
}
