
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

        public async Task AssignJudgeAsync(TournamentJudgeRequestDto requestDto)
        {
            var tournament = await _tournamentDao.GetTournamentByIdAsync(requestDto.TournamentID);
            if (tournament == null)
                throw new NotFoundException("El torneo no existe.");

            // Validar que el juez no este asignado
            var existingJudges = await _tournamentJudgeDao.GetJudgesByTournamentAsync(requestDto.TournamentID);
            if (existingJudges.Any(j => j == requestDto.JudgeID))
                throw new InvalidOperationException("El juez ya está asignado a este torneo.");

            var success = await _tournamentJudgeDao.AddJudgeToTournamentAsync(requestDto.TournamentID, requestDto.JudgeID);
        }

        public async Task RemoveJudgeAsync(int tournamentId, int judgeId)
        {
            var success = await _tournamentJudgeDao.RemoveJudgeFromTournamentAsync(tournamentId, judgeId);
        }
    }
}
