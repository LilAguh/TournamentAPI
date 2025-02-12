
using Dapper;
using DataAccess.Database;
using Models.DTOs.Decks;
using Services.Interfaces;

namespace Services.Implementations
{
    public class DeckDao : IDeckDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public DeckDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<int> CreateDeckAsync(DeckRequestDto dto, int userId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"INSERT INTO Decks (UserId, Name) 
                          VALUES (@UserId, @Name);
                          SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(query, new { UserId = userId, dto.Name });
        }

        public async Task<DeckResponseDto> GetDeckByIdAsync(int deckId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Decks WHERE Id = @DeckId";
            return await connection.QueryFirstOrDefaultAsync<DeckResponseDto>(query, new { DeckId = deckId });
        }

        public async Task<IEnumerable<DeckResponseDto>> GetDecksByUserAsync(int userId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Decks WHERE UserId = @UserId";
            return await connection.QueryAsync<DeckResponseDto>(query, new { UserId = userId });
        }

        public async Task<bool> DeleteDeckAsync(int deckId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM Decks WHERE Id = @DeckId";
            var rowsAffected = await connection.ExecuteAsync(query, new { DeckId = deckId });
            return rowsAffected > 0;
        }
    }
}
