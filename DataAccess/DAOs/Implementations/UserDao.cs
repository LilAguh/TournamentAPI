using Dapper;
using Models.DTOs.User;
using DataAccess.DAOs.Interfaces;
using Models.Entities;
using DataAccess.Database;
using Newtonsoft.Json;
using Models.Enums;

namespace DataAccess.DAOs.Implementations
{
    public class UserDao : IUserDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public UserDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<UserResponseDto> GetUserByAliasAsync(string alias)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" SELECT * FROM users WHERE Alias = @Alias";
            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { Alias = alias });
        }
        public async Task<User> GetActiveUserByEmailAsync(string email)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
                SELECT Id, Role, FirstName, LastName, Alias, Email, PasswordHash, 
                       CountryCode, AvatarUrl, CreatedAt, LastLogin, IsActive, CreatedBy
                FROM users
                WHERE Email = @Email AND IsActive = 1";
            return await connection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
        }
        public async Task<User> GetUserByIdentifierAsync(string identifier)
        {
            // Primero se intenta obtener por alias
            var user = await GetUserByAliasAsync(identifier);
            if (user != null)
                return user;
            // Si no se encuentra por alias, se busca por email (solo entre activos)
            return await GetActiveUserByEmailAsync(identifier);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
                SELECT Id, Role, FirstName, LastName, Alias, Email, PasswordHash, 
                       CountryCode, AvatarUrl, CreatedAt, LastLogin, IsActive, CreatedBy
                FROM users
                WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
        }

        public async Task AddUserAsync(User user)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
                INSERT INTO users 
                    (Role, FirstName, LastName, Alias, Email, PasswordHash, CountryCode, CreatedBy, CreatedAt, IsActive)
                VALUES 
                    (@Role, @FirstName, @LastName, @Alias, @Email, @PasswordHash, @CountryCode, @CreatedBy, @CreatedAt, @IsActive)";

            await connection.ExecuteAsync(query, user);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "UPDATE users SET LastLogin = @LastLogin WHERE id = @UserId";
                await connection.ExecuteAsync(query, new
                {
                    LastLogin = DateTime.UtcNow,
                    UserId = userId
                });
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
                UPDATE users 
                SET FirstName = @FirstName, 
                    LastName = @LastName,
                    Alias = @Alias,
                    Email = @Email,
                    PasswordHash = @PasswordHash,
                    CountryCode = @CountryCode, 
                    AvatarUrl = @AvatarUrl
                WHERE Id = @Id";

            await connection.ExecuteAsync(query, user);
        }

        public async Task UpdateUserStatusAsync(User user)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"UPDATE users SET IsActive = @IsActive WHERE Id = @Id";
            await connection.ExecuteAsync(query, new { user.IsActive, user.Id });
        }
    }
}