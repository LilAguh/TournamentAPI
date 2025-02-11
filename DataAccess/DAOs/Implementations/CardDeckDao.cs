
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.CardDecks;

namespace DataAccess.DAOs.Implementations
{
    public class CardDeckDao : ICardDeckDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public CardDeckDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<bool> AddCardToDeckAsync(int deckId, int cardId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "INSERT INTO CardDecks (DeckID, CardID) VALUES (@DeckId, @CardId)";
            var rowsAffected = await connection.ExecuteAsync(query, new { DeckId = deckId, CardId = cardId });
            return rowsAffected > 0;
        }

        public async Task<bool> RemoveCardFromDeckAsync(int deckId, int cardId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM CardDecks WHERE DeckID = @DeckId AND CardID = @CardId";
            var rowsAffected = await connection.ExecuteAsync(query, new { DeckId = deckId, CardId = cardId });
            return rowsAffected > 0;
        }

        public async Task<int> GetCardCountInDeckAsync(int deckId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT COUNT(*) FROM CardDecks WHERE DeckID = @DeckId";
            return await connection.ExecuteScalarAsync<int>(query, new { DeckId = deckId });
        }

        public async Task<IEnumerable<CardDeckResponseDto>> GetCardsInDeckAsync(int deckId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT c.Id AS CardId, c.Name AS CardName
                          FROM CardDecks cd
                          INNER JOIN Cards c ON cd.CardID = c.Id
                          WHERE cd.DeckID = @DeckId";
            return await connection.QueryAsync<CardDeckResponseDto>(query, new { DeckId = deckId });
        }
    }
}
