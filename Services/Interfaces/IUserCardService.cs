using Models.DTOs.UserCards;

namespace Services.Interfaces
{
    public interface IUserCardService
    {
        Task<bool> AddUserCardAsync(int userId, AddUserCardRequestDto dto);
    }
}
