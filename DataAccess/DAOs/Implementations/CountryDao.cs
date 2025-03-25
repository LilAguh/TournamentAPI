using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.Country;


namespace DataAccess.DAOs.Implementations
{
    public class CountryDao : ICountryDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public CountryDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        // Verifica si existe un país con el código especificado.
        // Retorna true si el país existe, de lo contrario false.
        public async Task<bool> CountryExists(string code)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "SELECT COUNT(1) FROM countries WHERE code = @Code";
                var count = await connection.ExecuteScalarAsync<int>(query, new { Code = code });
                return count > 0;
            }
        }

        // Obtiene la lista de todos los países registrados.
        // Retorna una colección de CountryResponseDto con el código y nombre de cada país.
        public async Task<IEnumerable<CountryResponseDto>> GetAllCountriesAsync()
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT Code, Name FROM countries";
            return await connection.QueryAsync<CountryResponseDto>(query);
        }
    }
}
