
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.Series;
using Dapper;

namespace DataAccess.DAOs.Implementations
{
    public class SerieDao : ISerieDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public SerieDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<SeriesResponseDto> AddSeriesAsync(SeriesRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var insertQuery = @"
            INSERT INTO Series (Name, CreatedAt)
            VALUES (@Name, @CreatedAt);
            SELECT LAST_INSERT_ID();";

            int newId = await connection.ExecuteScalarAsync<int>(insertQuery, new { dto.Name, dto.CreatedAt });
            var selectQuery = "SELECT * FROM Series WHERE ID = @Id";
            return await connection.QueryFirstOrDefaultAsync<SeriesResponseDto>(selectQuery, new { Id = newId });
        }

        public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesAsync()
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Series";
            return await connection.QueryAsync<SeriesResponseDto>(query);
        }

        public async Task<SeriesResponseDto?> GetSeriesByIdAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Series WHERE ID = @Id";
            return await connection.QueryFirstOrDefaultAsync<SeriesResponseDto>(query, new { Id = id });
        }

        public async Task<SeriesResponseDto?> UpdateSeriesAsync(int id, SeriesRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var updateQuery = @"
            UPDATE Series 
            SET Name = @Name, CreatedAt = @CreatedAt
            WHERE ID = @Id";
            var rowsAffected = await connection.ExecuteAsync(updateQuery, new { Id = id, dto.Name, dto.CreatedAt });
            if (rowsAffected > 0)
            {
                var selectQuery = "SELECT * FROM Series WHERE ID = @Id";
                return await connection.QueryFirstOrDefaultAsync<SeriesResponseDto>(selectQuery, new { Id = id });
            }
            return null;
        }

        public async Task<bool> DeleteSeriesAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM Series WHERE ID = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
