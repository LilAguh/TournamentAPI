using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Implementations
{
    public class UserDao : IUserDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public UserDao (IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<User> GetUserById(int id)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "SELECT * FROM Users WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
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

        public async Task AddUser(UserDto userDto)
        {
            try
            {
                using (var connection = await _databaseConnection.GetConnectionAsync())
                {
                    var query = @"
                INSERT INTO Users (Role, Name, LastName, Alias, Email, Password, Country, Avatar, Active, CreatedBy)
                VALUES (@Role, @Name, @LastName, @Alias, @Email, @Password, @Country, @Avatar, @Active, @CreatedBy)";
                    await connection.ExecuteAsync(query, userDto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting user into database:");
                Console.WriteLine(ex.Message);
            }
        }

        public async Task UpdateUser(int id, UserDto userDto)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = @"
            UPDATE Users
            SET Name = @Name, LastName = @LastName, Alias = @Alias, Email = @Email,
                Password = @Password, Country = @Country, Role = @Role,
                AvatarUrl = @AvatarUrl
            WHERE Id = @Id";

                var parameters = new
                {
                    Id = id,
                    userDto.Name,
                    userDto.LastName,
                    userDto.Alias,
                    userDto.Email,
                    userDto.Password,
                    userDto.Country,
                    userDto.Role,
                    userDto.Avatar
                };

                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeactivateUser(int id)
        {
            using (var connection = await _databaseConnection.GetConnectionAsync())
            {
                var query = "UPDATE Users SET Active = false WHERE Id = @Id";
                await connection.ExecuteAsync(query, new { Id = id });
            }
        }
    }
}
