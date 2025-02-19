
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;

namespace DataAccess.DAOs.Implementations
{
    public class TournamentPlayerDao : ITournamentPlayerDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public TournamentPlayerDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<bool> RegisterPlayerAsync(int tournamentId, int userId, int deckId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "INSERT INTO TournamentPlayers (TournamentID, UserID, DeckID) VALUES (@TournamentId, @UserId, @DeckId)";
            int rowsAffected = await connection.ExecuteAsync(query, new { TournamentId = tournamentId, UserId = userId, DeckId = deckId });
            return rowsAffected > 0;
        }

        public async Task<bool> IsPlayerRegisteredAsync(int tournamentId, int userId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT COUNT(*) FROM TournamentPlayers WHERE TournamentID = @TournamentId AND UserID = @UserId";
            int count = await connection.ExecuteScalarAsync<int>(query, new { TournamentId = tournamentId, UserId = userId });
            return count > 0;
        }

        public async Task<int> GetPlayerCountAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT COUNT(*) FROM TournamentPlayers WHERE TournamentID = @TournamentId";
            return await connection.ExecuteScalarAsync<int>(query, new { TournamentId = tournamentId });
        }
    }
}
