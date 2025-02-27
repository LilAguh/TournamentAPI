
namespace DataAccess.DAOs.Interfaces
{
    public interface ITournamentJudgeDao
    {
        Task<bool> AddJudgeToTournamentAsync(int tournamentId, int judgeId);
        Task<bool> RemoveJudgeFromTournamentAsync(int tournamentId, int judgeId);
        Task<IEnumerable<int>> GetJudgesByTournamentAsync(int tournamentId);
    }
}
