using Dapper;
using Models.DTOs;
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

        public async Task<UserResponseDto> GetUserByEmailOrAliasAsync(string emailOrAlias)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
                SELECT Id, Alias, Email, CountryCode, AvatarUrl, Role, CreatedAt
                FROM users
                WHERE Email = @EmailOrAlias OR Alias = @EmailOrAlias";

            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { EmailOrAlias = emailOrAlias });
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
                SELECT Id, Alias, FirstName, LastName, Email, CountryCode, PasswordHash, AvatarUrl, Role, CreatedAt, IsAvtive
                FROM users
                WHERE Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { Id = id });
        }

        public async Task AddUserAsync(UserCreateDto user)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
                INSERT INTO users 
                    (FirstName, LastName, Alias, Email, PasswordHash, CountryCode, Role, CreatedBy, CreatedAt, IsActive)
                VALUES 
                    (@FirstName, @LastName, @Alias, @Email, @PasswordHash, @CountryCode, @Role, @CreatedBy, @CreatedAt, @IsActive)";

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

        public async Task UpdateUserAsync(UserUpdateDto user)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
                UPDATE users 
                SET FirstName = @FirstName, 
                    LastName = @LastName, 
                    Alias = @Alias, 
                    Email = @Email, 
                    CountryCode = @CountryCode, 
                    AvatarUrl = @AvatarUrl,
                    PasswordHash = @PasswordHash,
                    IsActive = @IsActive
                WHERE Id = @Id";

            await connection.ExecuteAsync(query, user);
        }

        public async Task UpdateUserStatusAsync(int userId, bool isActive)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "UPDATE users SET IsActive = @IsActive WHERE Id = @UserId";
            await connection.ExecuteAsync(query, new { UserId = userId, IsActive = isActive });
        }
    }
}