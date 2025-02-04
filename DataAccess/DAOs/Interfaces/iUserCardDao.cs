
using Models.DTOs.UserCards;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserCardDao
    {
        Task<bool> AddUserCardAsync(int userId, AddUserCardRequestDto dto);
    }
}
