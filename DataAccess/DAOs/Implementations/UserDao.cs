using Dapper;
using Models.DTOs;
using DataAccess.DAOs.Interfaces;
using Models.Entities;
using DataAccess.Database;
using Newtonsoft.Json;

namespace DataAccess.DAOs.Implementations
{
    public class UserDao : IUserDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public UserDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task AddUserAsync(User user)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = @"
                INSERT INTO users (role, first_name, last_name, alias, email, password_hash, country_code, avatar, created_at, active, created_by)
                VALUES (@Role, @FirstName, @LastName, @Alias, @Email, @PasswordHash, @CountryCode, @AvatarUrl, @CreatedAt, @IsActive, @CreatedBy)";

                await connection.ExecuteAsync(query, user);
            }
        }


        public async Task<User> GetUserByEmailOrAliasAsync(string emailOrAlias)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = @"
        SELECT id, role, first_name, last_name, alias, email, password_hash, country_code, avatar, created_at, last_login, active, created_by
        FROM users
        WHERE email = @EmailOrAlias OR alias = @EmailOrAlias";

                // Ejecutar la consulta y obtener el resultado como un objeto dinámico
                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { EmailOrAlias = emailOrAlias });

                // Depurar el resultado
                if (result != null)
                {
                    Console.WriteLine($"Resultado de la consulta: {JsonConvert.SerializeObject(result)}");
                }

                // Mapear manualmente a la clase User
                if (result == null) return null;

                return new User
                {
                    Id = result.id,
                    Role = result.role,
                    FirstName = result.first_name,
                    LastName = result.last_name,
                    Alias = result.alias,
                    Email = result.email,
                    PasswordHash = result.password_hash, // Asegúrate de que coincida
                    CountryCode = result.country_code,
                    AvatarUrl = result.avatar,
                    CreatedAt = result.created_at,
                    LastLogin = result.last_login,
                    IsActive = result.active,
                    CreatedBy = result.created_by
                };
            }
        }

    }
}
