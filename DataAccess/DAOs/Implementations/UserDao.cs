using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs.Implementations
{
    public class UserDao : IUserDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public UserDao (IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<User> GetUserById (int id)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "SELECT * FROM Users WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id});
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "SELECT * FROM Users WHERE Email = @Email";
                return await connection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
            }
        }
    }
}
