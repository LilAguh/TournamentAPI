
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.CardSeries;
using Dapper;

namespace DataAccess.DAOs.Implementations
{
    public class CardSeriesDao : ICardSeriesDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public CardSeriesDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        // Agrega una carta a una serie específica.
        // Devuelve true si la operación fue exitosa, false en caso contrario.
        public async Task<bool> AddCardToSeriesAsync(AddCardSeriesRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"INSERT INTO CardSeries (CardID, SeriesID) VALUES (@CardId, @SeriesId)";
            var rowsAffected = await connection.ExecuteAsync(query, dto);
            return rowsAffected > 0;
        }

        // Elimina una carta de una serie específica.
        // Devuelve true si la operación fue exitosa, false en caso contrario.
        public async Task<bool> RemoveCardFromSeriesAsync(int cardId, int seriesId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"DELETE FROM CardSeries WHERE CardID = @CardId AND SeriesID = @SeriesId";
            var rowsAffected = await connection.ExecuteAsync(query, new { CardId = cardId, SeriesId = seriesId });
            return rowsAffected > 0;
        }

        // Obtiene todas las cartas asociadas a una serie específica.
        // Retorna una lista de objetos CardSeriesResponseDto con la información de la carta y la serie.
        public async Task<IEnumerable<CardSeriesResponseDto>> GetCardsBySeriesAsync(int seriesId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT c.Id AS CardId, c.Name AS CardName, s.Id AS SeriesId, s.Name AS SeriesName
                          FROM CardSeries cs
                          INNER JOIN Cards c ON cs.CardID = c.Id
                          INNER JOIN Series s ON cs.SeriesID = s.Id
                          WHERE cs.SeriesID = @SeriesId";
            return await connection.QueryAsync<CardSeriesResponseDto>(query, new { SeriesId = seriesId });
        }

        // Obtiene todas las series a las que pertenece una carta específica.
        // Retorna una lista de objetos CardSeriesResponseDto con la información de la serie y la carta.
        public async Task<IEnumerable<CardSeriesResponseDto>> GetSeriesByCardAsync(int cardId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT s.Id AS SeriesId, s.Name AS SeriesName, c.Id AS CardId, c.Name AS CardName
                          FROM CardSeries cs
                          INNER JOIN Series s ON cs.SeriesID = s.Id
                          INNER JOIN Cards c ON cs.CardID = c.Id
                          WHERE cs.CardID = @CardId";
            return await connection.QueryAsync<CardSeriesResponseDto>(query, new { CardId = cardId });
        }
    }
}
