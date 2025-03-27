
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

        // Agrega una carta al usuario.
        // Si ya existe el registro, incrementa la cantidad.
        public async Task<bool> AddUserCardAsync(int userId, AddUserCardRequestDto dto)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" INSERT INTO UserCards (UserID, CardID, Quantity)
                           VALUES (@UserId, @CardId, @Quantity)
                           ON DUPLICATE KEY UPDATE Quantity = Quantity + @Quantity";

            return await connection.ExecuteAsync(query, new
            {
                UserId = userId,
                dto.CardId,
                dto.Quantity
            }) > 0;
        }

        // Obtiene la lista de cartas del usuario.
        // Realiza un INNER JOIN para traer además el nombre de la carta.
        public async Task<IEnumerable<UserCardResponseDto>> GetUserCardsAsync(int userId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" SELECT c.Id AS CardId, c.Name AS CardName, uc.Quantity 
                           FROM UserCards uc
                           INNER JOIN Cards c ON uc.CardID = c.Id
                           WHERE uc.UserID = @UserId";
            return await connection.QueryAsync<UserCardResponseDto>(query, new { UserId = userId });
        }

        // Elimina una carta del usuario según su ID.
        public async Task<bool> RemoveUserCardAsync(int userId, int cardId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" DELETE FROM UserCards 
                           WHERE UserID = @UserId AND CardID = @CardId";
            return await connection.ExecuteAsync(query, new { UserId = userId, CardId = cardId }) > 0;
        }
    }
}
