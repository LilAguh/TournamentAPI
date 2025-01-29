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

        public async Task<User> GetUserByEmailOrAliasAsync(string emailOrAlias)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
        SELECT id, role, first_name, last_name, alias, email, password_hash, country_code, avatar, created_at, last_login, active, created_by
        FROM users
        WHERE email = @EmailOrAlias OR alias = @EmailOrAlias";

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { EmailOrAlias = emailOrAlias });

            if (result == null)
            {
                Console.WriteLine($"No se encontró ningún usuario con el alias o email: {emailOrAlias}");
            }
            else
            {
                Console.WriteLine($"Resultado crudo: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
            }

            // Mapear a User manualmente
            return result == null ? null : new User
            {
                Id = result.id,
                Role = Enum.Parse<RoleEnum>(result.role.ToString()),
                FirstName = result.first_name,
                LastName = result.last_name,
                Alias = result.alias,
                Email = result.email,
                PasswordHash = result.password_hash,
                CountryCode = result.country_code,
                AvatarUrl = result.avatar,
                CreatedAt = result.created_at,
                LastLogin = result.last_login,
                IsActive = result.active,
                CreatedBy = result.created_by
            };
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = @"
                SELECT id, role, first_name, last_name, alias, email, password_hash, country_code, avatar, created_at, last_login, active, created_by
                FROM users
                WHERE id = @Id";

                return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
            }
        }

        public async Task AddUserAsync(User user)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"
            INSERT INTO users 
            (role, first_name, last_name, alias, email, password_hash, country_code, created_by, created_at, active)
            VALUES 
            (@Role, @FirstName, @LastName, @Alias, @Email, @PasswordHash, @CountryCode, @CreatedBy, @CreatedAt, @IsActive)";
            await connection.ExecuteAsync(query, user);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "UPDATE users SET last_login = @LastLogin WHERE id = @UserId";
                await connection.ExecuteAsync(query, new
                {
                    LastLogin = DateTime.UtcNow,
                    UserId = userId
                });
            }
        }
    }
}