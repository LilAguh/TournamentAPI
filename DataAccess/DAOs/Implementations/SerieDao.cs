
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

        // Agrega una nueva serie a la base de datos y retorna el objeto SeriesResponseDto de la serie creada.
        public async Task<SeriesResponseDto> AddSeriesAsync(SeriesRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"INSERT INTO Series (Name, CreatedAt) 
                          VALUES (@Name, @CreatedAt);
                          SELECT LAST_INSERT_ID();";
            var id = await connection.ExecuteScalarAsync<int>(query, dto);
            return await GetSeriesByIdAsync(id);
        }

        // Retorna todas las series registradas en la base de datos.
        public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesAsync()
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Series";
            return await connection.QueryAsync<SeriesResponseDto>(query);
        }

        // Retorna una serie específica por su ID.
        public async Task<SeriesResponseDto> GetSeriesByIdAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Series WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<SeriesResponseDto>(query, new { Id = id });
        }

        // Actualiza los datos de una serie y retorna el objeto actualizado.
        public async Task<SeriesResponseDto> UpdateSeriesAsync(int id, SeriesRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"UPDATE Series SET 
                          Name = @Name, CreatedAt = @CreatedAt 
                          WHERE Id = @Id";
            await connection.ExecuteAsync(query, new { Id = id, dto.Name, dto.CreatedAt });
            return await GetSeriesByIdAsync(id);
        }

        // Elimina una serie de la base de datos y retorna true si la eliminación fue exitosa.
        public async Task<bool> DeleteSeriesAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM Series WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
