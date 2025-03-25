using Dapper;
using Models.DTOs.User;
using DataAccess.DAOs.Interfaces;
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

        // Obtiene un usuario activo por su alias.
        public async Task<UserResponseDto> GetUserByAliasAsync(string alias)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM users WHERE Alias = @Alias AND IsActive = 1";
            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { Alias = alias });
        }

        // Obtiene un usuario activo por su email.
        public async Task<UserResponseDto> GetActiveUserByEmailAsync(string email)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM users WHERE Email = @Email AND IsActive = 1";
            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { Email = email });
        }

        // Intenta obtener un usuario activo primero por alias; si no lo encuentra, lo busca por email.
        public async Task<UserResponseDto> GetUserByIdentifierAsync(string identifier)
        {
            var user = await GetUserByAliasAsync(identifier);
            if (user != null)
                return user;
            return await GetActiveUserByEmailAsync(identifier);
        }

        // Obtiene un usuario por su ID.
        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM users WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<UserResponseDto>(query, new { Id = id });
        }

        // Agrega un nuevo usuario y retorna el ID generado.
        public async Task<int> AddUserAsync(UserRequestDto user)
        {
            var query = @"
                INSERT INTO Users (FirstName, LastName, Alias, Email, PasswordHash, CountryCode, AvatarUrl, Role, CreatedBy, CreatedAt, IsActive)
                VALUES (@FirstName, @LastName, @Alias, @Email, @PasswordHash, @CountryCode, @AvatarUrl, @Role, @CreatedBy, @CreatedAt, @IsActive);
                SELECT LAST_INSERT_ID();";

            using var connection = await _databaseConnection.GetConnectionAsync();
            return await connection.ExecuteScalarAsync<int>(query, user);
        }

        // Actualiza la fecha de último inicio de sesión para un usuario.
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

        // Actualiza los datos de un usuario existente.
        public async Task UpdateUserAsync(UserResponseDto userDto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"UPDATE users 
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

        // Actualiza el estado (activo/inactivo) de un usuario.
        public async Task UpdateUserStatusAsync(UserResponseDto userDto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "UPDATE users SET IsActive = @IsActive WHERE Id = @Id";
            await connection.ExecuteAsync(query, new { userDto.IsActive, userDto.Id });
        }

        // Elimina permanentemente un usuario de la base de datos.
        public async Task<bool> PermanentDeleteUserAsync(int userId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM Users WHERE ID = @Id";
            return await connection.ExecuteAsync(query, new { Id = userId }) > 0;
        }
    }
}