
using Models.DTOs.UserCards;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserCardDao
    {
        Task<bool> AddUserCardAsync(int userId, AddUserCardRequestDto dto);
        Task<IEnumerable<UserCardResponseDto>> GetUserCardsAsync(int userId);
        Task<bool> RemoveUserCardAsync(int userId, int cardId);
    }
}
