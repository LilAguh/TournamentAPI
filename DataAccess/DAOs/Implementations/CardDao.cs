
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.Cards;

namespace DataAccess.DAOs.Implementations
{
    public class CardDao : ICardDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public CardDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<int> AddCardAsync(CardRequestDto card, int adminId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
            INSERT INTO Cards (Name, Attack, Defense, IllustrationUrl, CreatedBy) 
            VALUES (@Name, @Attack, @Defense, @IllustrationUrl, @CreatedBy);
            SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                card.Name,
                card.Attack,
                card.Defense,
                card.IllustrationUrl,
                CreatedBy = adminId
            });
        }

        public async Task<IEnumerable<CardResponseDto>> GetAllCardsAsync()
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Cards";
            return await connection.QueryAsync<CardResponseDto>(query);
        }

        public async Task<CardResponseDto> GetCardByIdAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Cards WHERE ID = @Id";
            return await connection.QueryFirstOrDefaultAsync<CardResponseDto>(query, new { Id = id });
        }

        public async Task<bool> UpdateCardAsync(int id, CardRequestDto card)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
            UPDATE Cards 
            SET Name = @Name, Attack = @Attack, Defense = @Defense, IllustrationUrl = @IllustrationUrl
            WHERE ID = @Id";

            var rowsAffected = await connection.ExecuteAsync(query, new
            {
                Id = id,
                card.Name,
                card.Attack,
                card.Defense,
                card.IllustrationUrl
            });
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCardAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM Cards WHERE ID = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
