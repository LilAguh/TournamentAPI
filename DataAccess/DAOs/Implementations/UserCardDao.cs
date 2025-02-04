
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.UserCards;

namespace DataAccess.DAOs.Implementations
{
    public class UserCardDao : IUserCardDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public UserCardDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<bool> AddUserCardAsync(int userId, AddUserCardRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();

            // Primero verificamos si el usuario ya posee la carta
            var queryCheck = "SELECT Quantity FROM UserCards WHERE UserID = @UserId AND CardID = @CardId";
            var existingQuantity = await connection.QueryFirstOrDefaultAsync<int?>(queryCheck, new { UserId = userId, CardId = dto.CardId });

            if (existingQuantity.HasValue)
            {
                // Si ya existe, actualizamos la cantidad (sumamos la nueva cantidad)
                var queryUpdate = "UPDATE UserCards SET Quantity = Quantity + @Quantity WHERE UserID = @UserId AND CardID = @CardId";
                var rowsAffected = await connection.ExecuteAsync(queryUpdate, new { UserId = userId, CardId = dto.CardId, Quantity = dto.Quantity });
                return rowsAffected > 0;
            }
            else
            {
                // Si no existe, insertamos un nuevo registro
                var queryInsert = "INSERT INTO UserCards (UserID, CardID, Quantity) VALUES (@UserId, @CardId, @Quantity)";
                var rowsAffected = await connection.ExecuteAsync(queryInsert, new { UserId = userId, CardId = dto.CardId, Quantity = dto.Quantity });
                return rowsAffected > 0;
            }
        }
    }
}
