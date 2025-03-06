
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Matches;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class MatchService : IMatchService
    {
        private readonly IMatchDao _matchDao;
        private readonly ITournamentPlayerDao _tournamentPlayerDao;
        private readonly ITournamentDao _tournamentDao;

        public MatchService(IMatchDao matchDao, ITournamentPlayerDao tournamentPlayerDao, ITournamentDao tournamentDao)
        {
            _matchDao = matchDao;
            _tournamentPlayerDao = tournamentPlayerDao;
            _tournamentDao = tournamentDao;
        }

        public async Task CreateRoundMatchAsync(int tournamentId)
        {
            // Obtener la lista de IDs de jugadores inscritos en el torneo
            var playerIds = await _tournamentPlayerDao.GetPlayerIdsAsync(tournamentId);

            // Obtener la última ronda generada para el torneo
            int lastRound = await _matchDao.GetLastRoundAsync(tournamentId);

            var tournament = await _tournamentDao.GetTournamentByIdAsync(tournamentId);

            // Calcular la siguiente ronda
            int nextRound = lastRound == 0 ? tournament.MaxPlayers / 2 : lastRound / 2;

            if (lastRound == 1)
            {
                throw new ForbiddenException("No se pueden generar más partidos. Ya se jugo la final.");
            }

            // Llamar al DAO de matches para crear los partidos de la ronda
            await _matchDao.CreateRoundMatchAsync(tournamentId, playerIds, nextRound);
        }

        public async Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId)
        {
            return await _matchDao.GetMatchesByTournamentAsync(tournamentId);
        }

        public async Task<bool> UpdateMatchWinnerAsync(int matchId, int winnerId)
        {
            // Obtener el partido para validar que el ganador participe
            var match = await _matchDao.GetMatchByIdAsync(matchId);
            if (match == null)
                throw new NotFoundException("Partido no encontrado");

            // Validar que el winnerId sea uno de los jugadores en el partido
            if (match.Player1ID != winnerId && match.Player2ID != winnerId)
                throw new ValidationException("El jugador ganador no participó en este partido");

            // Actualizamos el partido
            bool updated = await _matchDao.UpdateMatchWinnerAsync(matchId, winnerId);

            // Si el partido es de la ronda final (Round == 1), finalizamos el torneo
            if (match.Round == 1)
                await _tournamentDao.FinalizeTournamentAsync(match.TournamentID, winnerId);

            return updated;
        }
    }
}
