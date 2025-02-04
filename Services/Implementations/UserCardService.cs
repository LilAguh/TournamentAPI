
using DataAccess.DAOs.Interfaces;
using Models.DTOs.UserCards;

namespace Services.Implementations
{
    public class UserCardService
    {
        private readonly IUserCardDao _userCardDao;

        public UserCardService(IUserCardDao userCardDao)
        {
            _userCardDao = userCardDao;
        }

        public async Task<bool> AddUserCardAsync(int userId, AddUserCardRequestDto dto)
        {
            return await _userCardDao.AddUserCardAsync(userId, dto);
        }
        public async Task<IEnumerable<UserCardResponseDto>> GetUserCardsAsync(int userId)
        {
            return await _userCardDao.GetUserCardsAsync(userId);
        }

        public async Task<bool> RemoveUserCardAsync(int userId, int cardId)
        {
            return await _userCardDao.RemoveUserCardAsync(userId, cardId);
        }
    }
}
