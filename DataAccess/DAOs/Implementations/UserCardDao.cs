
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

        public async Task<IEnumerable<UserCardResponseDto>> GetUserCardsAsync(int userId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            // Realizamos un JOIN para obtener información adicional de la carta
            var query = @"
            SELECT uc.CardID, c.Name as CardName, uc.Quantity 
            FROM UserCards uc
            INNER JOIN Cards c ON uc.CardID = c.ID
            WHERE uc.UserID = @UserId";
            return await connection.QueryAsync<UserCardResponseDto>(query, new { UserId = userId });
        }

        public async Task<bool> RemoveUserCardAsync(int userId, int cardId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();

            // Primero obtenemos la cantidad actual para esa carta
            var querySelect = "SELECT Quantity FROM UserCards WHERE UserID = @UserId AND CardID = @CardId";
            var quantity = await connection.QueryFirstOrDefaultAsync<int?>(querySelect, new { UserId = userId, CardId = cardId });

            if (!quantity.HasValue)
            {
                // No se encontró la carta en el registro del usuario
                return false;
            }

            if (quantity > 1)
            {
                // Si tiene más de 1, se decrementa en 1
                var queryUpdate = "UPDATE UserCards SET Quantity = Quantity - 1 WHERE UserID = @UserId AND CardID = @CardId";
                var rowsAffected = await connection.ExecuteAsync(queryUpdate, new { UserId = userId, CardId = cardId });
                return rowsAffected > 0;
            }
            else
            {
                // Si tiene 1, se elimina la fila
                var queryDelete = "DELETE FROM UserCards WHERE UserID = @UserId AND CardID = @CardId";
                var rowsAffected = await connection.ExecuteAsync(queryDelete, new { UserId = userId, CardId = cardId });
                return rowsAffected > 0;
            }
        }
    }
}
