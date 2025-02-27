
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Disqualifications;
using Models.Entities;
using Services.Interfaces;

namespace Services.Implementations
{
    public class DisqualificationService : IDisqualificationService
    {
        private readonly IDisqualificationDao _disqualificationDao;
        private readonly ITournamentPlayerDao _tournamentPlayerDao;
        // Si es necesario, inyecta también otros DAOs (por ejemplo, para obtener el estado del torneo)

        public DisqualificationService(
            IDisqualificationDao disqualificationDao,
            ITournamentPlayerDao tournamentPlayerDao)
        {
            _disqualificationDao = disqualificationDao;
            _tournamentPlayerDao = tournamentPlayerDao;
        }

        public async Task<int> DisqualifyPlayerAsync(DisqualificationRequestDto requestDto, int judgeId)
        {
            // Verificar que el jugador esté registrado en el torneo.
            bool isRegistered = await _tournamentPlayerDao.IsPlayerRegisteredAsync(requestDto.TournamentID, requestDto.PlayerID);
            if (!isRegistered)
            {
                throw new InvalidOperationException("El jugador no está registrado en el torneo.");
            }

            // Verificar que el jugador no haya sido ya descalificado en este torneo.
            var disqualifications = await _disqualificationDao.GetDisqualificationsByTournamentAsync(requestDto.TournamentID);
            if (disqualifications.Any(d => d.PlayerID == requestDto.PlayerID))
            {
                throw new InvalidOperationException("El jugador ya ha sido descalificado en este torneo.");
            }

            // Aquí podrías agregar validaciones adicionales, por ejemplo:
            // - Comprobar el estado del torneo.
            // - Verificar restricciones de tiempo (por ejemplo, que el partido ya haya iniciado).

            var disqualification = new Disqualification
            {
                TournamentID = requestDto.TournamentID,
                PlayerID = requestDto.PlayerID,
                JudgeID = judgeId,
                Reason = requestDto.Reason
            };

            return await _disqualificationDao.AddDisqualificationAsync(disqualification);
        }
    }
}
