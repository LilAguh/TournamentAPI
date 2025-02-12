
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
            var query = @"INSERT INTO Series (Name, CreatedAt) 
                          VALUES (@Name, @CreatedAt);
                          SELECT LAST_INSERT_ID();";
            var id = await connection.ExecuteScalarAsync<int>(query, dto);
            return await GetSeriesByIdAsync(id);
        }

        public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesAsync()
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Series";
            return await connection.QueryAsync<SeriesResponseDto>(query);
        }

        public async Task<SeriesResponseDto> GetSeriesByIdAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Series WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<SeriesResponseDto>(query, new { Id = id });
        }

        public async Task<SeriesResponseDto> UpdateSeriesAsync(int id, SeriesRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"UPDATE Series SET 
                          Name = @Name, CreatedAt = @CreatedAt 
                          WHERE Id = @Id";
            await connection.ExecuteAsync(query, new { Id = id, dto.Name, dto.CreatedAt });
            return await GetSeriesByIdAsync(id);
        }

        public async Task<bool> DeleteSeriesAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM Series WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
