
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.Matches;

namespace DataAccess.DAOs.Implementations
{
    public class MatchDao : IMatchDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public MatchDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task CreateMatchesAsync(List<MatchCreateRequestDto> matches)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"INSERT INTO Matches 
                        (TournamentID, MatchNumber, TotalMatches, Player1ID, Player2ID, 
                         MatchStartTime, Status, Round)
                        VALUES (@TournamentId, @MatchNumber, @TotalMatches, @Player1Id, 
                                @Player2Id, @MatchStartTime, @Status, @Round)";

            await connection.ExecuteAsync(query, matches);
        }

        public async Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT 
                        Id,
                        TournamentID,
                        MatchNumber,
                        TotalMatches,
                        Player1ID,
                        Player2ID,
                        MatchStartTime,
                        Status,
                        Round,
                        WinnerID
                        FROM Matches 
                        WHERE TournamentID = @TournamentId
                        ORDER BY MatchStartTime";

            return await connection.QueryAsync<MatchResponseDto>(query, new { TournamentId = tournamentId });
        }

        public async Task<MatchResponseDto> GetMatchByIdAsync(int matchId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT 
                        Id,
                        TournamentID,
                        MatchNumber,
                        TotalMatches,
                        Player1ID,
                        Player2ID,
                        MatchStartTime,
                        Status,
                        Round,
                        WinnerID
                        FROM Matches 
                        WHERE ID = @MatchId";

            return await connection.QueryFirstOrDefaultAsync<MatchResponseDto>(query, new { MatchId = matchId });
        }

        public async Task<bool> UpdateMatchResultAsync(MatchResultRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"UPDATE Matches 
                        SET WinnerID = @WinnerId, 
                            Status = 'Finished'
                        WHERE ID = @MatchId";

            var rowsAffected = await connection.ExecuteAsync(query, dto);
            return rowsAffected > 0;
        }

        public async Task<List<int>> GetWinnersByRoundAsync(int tournamentId, int round)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT WinnerID 
                        FROM Matches 
                        WHERE TournamentID = @TournamentId 
                          AND Round = @Round
                          AND WinnerID IS NOT NULL";

            var result = await connection.QueryAsync<int>(query, new
            {
                TournamentId = tournamentId,
                Round = round
            });

            return result.AsList();
        }

        public async Task<int> GetCurrentMaxRoundAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT COALESCE(MAX(Round), 0) 
                        FROM Matches 
                        WHERE TournamentID = @TournamentId";

            return await connection.ExecuteScalarAsync<int>(query, new { TournamentId = tournamentId });
        }
    }
}
