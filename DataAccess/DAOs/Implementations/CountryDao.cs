using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;


namespace DataAccess.DAOs.Implementations
{
    public class CountryDao : ICountryDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public CountryDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<bool> CountryExists(string code)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "SELECT COUNT(1) FROM countries WHERE code = @Code";
                var count = await connection.ExecuteScalarAsync<int>(query, new { Code = code });
                return count > 0;
            }
        }
    }
}
