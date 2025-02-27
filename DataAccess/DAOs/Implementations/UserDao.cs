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
            var query = "SELECT * FROM users WHERE Alias = @Alias AND IsActive = 1";
            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { Alias = alias });
        }
        public async Task<UserResponseDto> GetActiveUserByEmailAsync(string email)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM users WHERE Email = @Email AND IsActive = 1";
            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { Email = email });
        }
        public async Task<UserResponseDto> GetUserByIdentifierAsync(string identifier)
        {
            // Primero se intenta obtener por alias
            var user = await GetUserByAliasAsync(identifier);
            if (user != null)
                return user;
            // Si no se encuentra por alias, se busca por email
            return await GetActiveUserByEmailAsync(identifier);
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM users WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { Id = id });
        }

        public async Task AddUserAsync(UserRequestDto userDto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" INSERT INTO users (Role, FirstName, LastName, Alias, Email, PasswordHash, CountryCode, CreatedBy, CreatedAt, IsActive)
                           VALUES (@Role, @FirstName, @LastName, @Alias, @Email, @PasswordHash, @CountryCode, @CreatedBy, @CreatedAt, @IsActive)";

            await connection.ExecuteAsync(query, userDto);
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

        public async Task UpdateUserAsync(UserResponseDto userDto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" UPDATE users 
                           SET FirstName = @FirstName, 
                           LastName = @LastName,
                           Alias = @Alias,
                           Email = @Email,
                           PasswordHash = @PasswordHash,
                           CountryCode = @CountryCode, 
                           AvatarUrl = @AvatarUrl
                           WHERE Id = @Id";

            await connection.ExecuteAsync(query, userDto);
        }

        public async Task UpdateUserStatusAsync(UserResponseDto userDto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "UPDATE users SET IsActive = @IsActive WHERE Id = @Id";
            await connection.ExecuteAsync(query, new { userDto.IsActive, userDto.Id });
        }

        //Solo va a funcionar para el testing
        public async Task<bool> PermanentDeleteUserAsync(int userId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM Users WHERE ID = @Id";
            return await connection.ExecuteAsync(query, new { Id = userId }) > 0;
        }
    }
}