
using Models.DTOs.TournamentJudges;

namespace Services.Interfaces
{
    public interface ITournamentJudgeService
    {
        Task AssignJudgeAsync(TournamentJudgeRequestDto requestDto);
        Task RemoveJudgeAsync(int tournamentId, int judgeId);
    }
}
