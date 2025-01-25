using Dapper;
using Models.DTOs;
using DataAccess.DAOs.Interfaces;
using Models.Entities;
using DataAccess.Database;

namespace DataAccess.DAOs.Implementations
{
    public class UserDao : IUserDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public UserDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task AddUserAsync(CreateUserDto userDto)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = @"
                INSERT INTO users (role, first_name, last_name, alias, email, password_hash, country_code, avatar, created_at, active, created_by)
                VALUES (@Role, @FirstName, @LastName, @Alias, @Email, @PasswordHash, @CountryCode, @AvatarUrl, @CreatedAt, @IsActive, @CreatedBy)";

                await connection.ExecuteAsync(query, userDto);
            }
        }


        public async Task<User> GetUserByEmailOrAliasAsync(string emailOrAlias)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "SELECT * FROM users WHERE email = @EmailOrAlias OR alias = @EmailOrAlias";
                return await connection.QueryFirstOrDefaultAsync<User>(query, new { EmailOrAlias = emailOrAlias });
            }
        }

    }
}
