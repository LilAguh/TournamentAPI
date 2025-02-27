
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;

namespace DataAccess.DAOs.Implementations
{
    public class TournamentJudgeDao : ITournamentJudgeDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public TournamentJudgeDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }
        public async Task<bool> AddJudgeToTournamentAsync(int tournamentId, int judgeId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "INSERT INTO TournamentJudges (TournamentID, JudgeID) VALUES (@TournamentID, @JudgeID)";
            var rowsAffected = await connection.ExecuteAsync(query, new { TournamentID = tournamentId, JudgeID = judgeId });
            return rowsAffected > 0;
        }

        public async Task<bool> RemoveJudgeFromTournamentAsync(int tournamentId, int judgeId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM TournamentJudges WHERE TournamentID = @TournamentID AND JudgeID = @JudgeID";
            var rowsAffected = await connection.ExecuteAsync(query, new { TournamentID = tournamentId, JudgeID = judgeId });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<int>> GetJudgesByTournamentAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT JudgeID FROM TournamentJudges WHERE TournamentID = @TournamentID";
            return await connection.QueryAsync<int>(query, new { TournamentID = tournamentId });
        }
    }
}
