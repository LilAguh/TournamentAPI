
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

            // Verificar que la carta no esté ya en el mazo
            var checkQuery = "SELECT COUNT(*) FROM CardDecks WHERE DeckID = @DeckId AND CardID = @CardId";
            int count = await connection.ExecuteScalarAsync<int>(checkQuery, new { DeckId = deckId, CardId = cardId });
            if (count > 0)
                return false; // Ya existe

            // Verificar que el mazo tenga menos de 15 cartas
            int currentCount = await GetCardCountInDeckAsync(deckId);
            if (currentCount >= 15)
                return false; // Límite alcanzado

            var insertQuery = "INSERT INTO CardDecks (DeckID, CardID) VALUES (@DeckId, @CardId)";
            int rowsAffected = await connection.ExecuteAsync(insertQuery, new { DeckId = deckId, CardId = cardId });
            return rowsAffected > 0;
        }

        public async Task<bool> RemoveCardFromDeckAsync(int deckId, int cardId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var deleteQuery = "DELETE FROM CardDecks WHERE DeckID = @DeckId AND CardID = @CardId";
            int rowsAffected = await connection.ExecuteAsync(deleteQuery, new { DeckId = deckId, CardId = cardId });
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
            // Realizamos JOIN con la tabla Cards para obtener el nombre de la carta
            var query = @"
            SELECT cd.DeckID, cd.CardID, c.Name AS CardName
            FROM CardDecks cd
            INNER JOIN Cards c ON cd.CardID = c.ID
            WHERE cd.DeckID = @DeckId";
            return await connection.QueryAsync<CardDeckResponseDto>(query, new { DeckId = deckId });
        }
    }
}
