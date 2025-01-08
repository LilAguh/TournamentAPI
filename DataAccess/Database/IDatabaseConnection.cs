using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Database
{
    public interface IDatabaseConnection
    {
        Task<MySqlConnection> GetConnectionAsync();
    }
}
