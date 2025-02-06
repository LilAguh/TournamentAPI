
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

        public async Task<bool> AddCardToSeriesAsync(AddCardSeriesRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            // Verificar si la relación ya existe
            var checkQuery = "SELECT COUNT(*) FROM CardSeries WHERE CardID = @CardId AND SeriesID = @SeriesId";
            int count = await connection.ExecuteScalarAsync<int>(checkQuery, new { dto.CardId, dto.SeriesId });
            if (count > 0) return false; // Ya asignada

            var insertQuery = "INSERT INTO CardSeries (CardID, SeriesID) VALUES (@CardId, @SeriesId)";
            int rowsAffected = await connection.ExecuteAsync(insertQuery, new { dto.CardId, dto.SeriesId });
            return rowsAffected > 0;
        }

        public async Task<bool> RemoveCardFromSeriesAsync(int cardId, int seriesId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var deleteQuery = "DELETE FROM CardSeries WHERE CardID = @CardId AND SeriesID = @SeriesId";
            int rowsAffected = await connection.ExecuteAsync(deleteQuery, new { CardId = cardId, SeriesId = seriesId });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<CardSeriesResponseDto>> GetCardsBySeriesAsync(int seriesId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            // JOIN con Cards para obtener el nombre de la carta y con Series para obtener el nombre de la serie
            var query = @"
            SELECT cs.CardID, cs.SeriesID, c.Name AS CardName, s.Name AS SeriesName
            FROM CardSeries cs
            INNER JOIN Cards c ON cs.CardID = c.ID
            INNER JOIN Series s ON cs.SeriesID = s.ID
            WHERE cs.SeriesID = @SeriesId";
            return await connection.QueryAsync<CardSeriesResponseDto>(query, new { SeriesId = seriesId });
        }

        public async Task<IEnumerable<CardSeriesResponseDto>> GetSeriesByCardAsync(int cardId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
            SELECT cs.CardID, cs.SeriesID, c.Name AS CardName, s.Name AS SeriesName
            FROM CardSeries cs
            INNER JOIN Cards c ON cs.CardID = c.ID
            INNER JOIN Series s ON cs.SeriesID = s.ID
            WHERE cs.CardID = @CardId";
            return await connection.QueryAsync<CardSeriesResponseDto>(query, new { CardId = cardId });
        }
    }
}
