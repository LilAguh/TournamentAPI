
using DataAccess.DAOs.Interfaces;
using Models.DTOs.TournamentJudges;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class TournamentJudgeService : ITournamentJudgeService
    {
        private readonly ITournamentJudgeDao _tournamentJudgeDao;
        private readonly ITournamentDao _tournamentDao;

        public TournamentJudgeService(ITournamentJudgeDao tournamentJudgeDao, ITournamentDao tournamentDao)
        {
            _tournamentJudgeDao = tournamentJudgeDao;
            _tournamentDao = tournamentDao;
        }

        // Asigna un juez a un torneo, si el torneo existe y el juez no está asignado previamente.
        public async Task AssignJudgeAsync(TournamentJudgeRequestDto requestDto)
        {
            var tournament = await _tournamentDao.GetTournamentByIdAsync(requestDto.TournamentID);
            if (tournament == null)
                throw new NotFoundException("El torneo no existe.");

            // Validar que el juez no esté asignado
            var existingJudges = await _tournamentJudgeDao.GetJudgesByTournamentAsync(requestDto.TournamentID);
            if (existingJudges.Any(j => j == requestDto.JudgeID))
                throw new InvalidOperationException("El juez ya está asignado a este torneo.");

            var success = await _tournamentJudgeDao.AddJudgeToTournamentAsync(requestDto.TournamentID, requestDto.JudgeID);
        }

        // Elimina un juez de un torneo.
        public async Task RemoveJudgeAsync(int tournamentId, int judgeId)
        {
            var success = await _tournamentJudgeDao.RemoveJudgeFromTournamentAsync(tournamentId, judgeId);
        }
    }
}
