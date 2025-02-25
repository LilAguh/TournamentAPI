
using DataAccess.DAOs.Implementations;
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Matches;
using Models.Exceptions;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class MatchService : IMatchService
    {
        private readonly IMatchDao _matchDao;
        private readonly ITournamentPlayerDao _tournamentPlayerDao;

        public MatchService(IMatchDao matchDao, ITournamentPlayerDao tournamentPlayerDao)
        {
            _matchDao = matchDao;
            _tournamentPlayerDao = tournamentPlayerDao;
        }

        public async Task CreateRoundMatchAsync(int tournamentId)
        {
            // Obtener la lista de IDs de jugadores inscritos en el torneo
            var playerIds = await _tournamentPlayerDao.GetPlayerIdsAsync(tournamentId);

            // Validaciones adicionales: asegurarse de que la cantidad de jugadores sea par, etc.
            if (playerIds.Count % 2 != 0)
            {
                throw new ValidationException("La cantidad de jugadores debe ser par para formar los partidos");
            }

            // Llamar al DAO de matches para crear los partidos de la ronda
            await _matchDao.CreateRoundMatchAsync(tournamentId, playerIds);
        }

        public async Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId)
        {
            return await _matchDao.GetMatchesByTournamentAsync(tournamentId);
        }
    }
}
