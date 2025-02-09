using Models.DTOs.UserCards;

namespace Services.Interfaces
{
    public interface IUserCardService
    {
        Task AddUserCardAsync(int userId, AddUserCardRequestDto dto);
        Task<IEnumerable<UserCardResponseDto>> GetUserCardsAsync(int userId);
        Task<bool> RemoveUserCardAsync(int userId, int cardId);
    }
}
