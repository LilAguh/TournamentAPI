using MySqlConnector;

namespace DataAccess.Database
{
    public class MySqlConnectionFactory : IDatabaseConnection
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<MySqlConnection> GetConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
