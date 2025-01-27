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

                // Convertir el rol de string a RoleEnum
                if (!Enum.TryParse(result.role.ToString(), out RoleEnum role))
                {
                    throw new InvalidOperationException($"Rol no válido: {result.role}");
                }

                return new User
                {
                    Id = result.id,
                    Role = role, // Asignar el RoleEnum convertido
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
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = @"
        INSERT INTO users 
            (role, first_name, last_name, alias, email, password_hash, country_code, created_by, created_at, active)
        VALUES 
            (@Role, @FirstName, @LastName, @Alias, @Email, @PasswordHash, @CountryCode, @CreatedBy, @CreatedAt, @IsActive)";

                await connection.ExecuteAsync(query, new
                {
                    Role = (int)user.Role, // Convertir Enum a int
                    user.FirstName,
                    user.LastName,
                    user.Alias,
                    user.Email,
                    user.PasswordHash,
                    user.CountryCode,
                    user.CreatedBy,
                    user.CreatedAt,
                    IsActive =  1
                });
            }
        }
    }
}