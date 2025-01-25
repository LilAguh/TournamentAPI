using MySqlConnector;

namespace DataAccess.Database
{
    public interface IDatabaseConnection
    {
        Task<MySqlConnection> GetConnectionAsync();
    }
}