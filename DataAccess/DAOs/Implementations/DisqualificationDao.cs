
using Dapper;
using DataAccess.Database;
using Models.DTOs.Disqualifications;
using Models.Entities;

namespace DataAccess.DAOs.Implementations
{
    public class DisqualificationDao : IDisqualificationDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public DisqualificationDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<int> AddDisqualificationAsync(Disqualification disqualification)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" INSERT INTO Disqualifications (TournamentID, PlayerID, JudgeID, Reason, CreatedAt)
                           VALUES (@TournamentID, @PlayerID, @JudgeID, @Reason, @CreatedAt);
                           SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(query, disqualification);
        }

        public async Task<IEnumerable<DisqualificationResponseDto>> GetDisqualificationsByTournamentAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" SELECT Id, TournamentID, PlayerID, JudgeID, Reason, CreatedAt 
                           FROM Disqualifications 
                           WHERE TournamentID = @TournamentID";
            return await connection.QueryAsync<DisqualificationResponseDto>(query, new { TournamentID = tournamentId });
        }
    }
}
