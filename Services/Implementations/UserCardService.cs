
using Config;
using DataAccess.DAOs.Interfaces;
using Models.DTOs.UserCards;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class UserCardService : IUserCardService
    {
        private readonly IUserCardDao _userCardDao;
        private readonly ICardDao _cardDao;
        private readonly IUserDao _userDao;

        public UserCardService(IUserCardDao userCardDao, ICardDao cardDao, IUserDao userDao)
        {
            _userCardDao = userCardDao;
            _cardDao = cardDao;
            _userDao = userDao;
        }

        // Agrega una carta al usuario después de validar que el usuario y la carta existen.
        public async Task AddUserCardAsync(int userId, AddUserCardRequestDto dto)
        {
            await ValidateUserExistence(userId);
            await ValidateCardExistence(dto.CardId);
            await _userCardDao.AddUserCardAsync(userId, dto);
        }

        // Retorna todas las cartas asociadas al usuario.
        public async Task<IEnumerable<UserCardResponseDto>> GetUserCardsAsync(int userId)
        {
            await ValidateUserExistence(userId);
            return await _userCardDao.GetUserCardsAsync(userId);
        }

        // Elimina una carta del usuario después de validar que el usuario y la carta existen.
        public async Task RemoveUserCardAsync(int userId, int cardId)
        {
            await ValidateUserExistence(userId);
            await ValidateCardExistence(cardId);
            await _userCardDao.RemoveUserCardAsync(userId, cardId);
        }

        // Métodos Privados

        // Valida que el usuario exista y esté activo.
        private async Task ValidateUserExistence(int userId)
        {
            var user = await _userDao.GetUserByIdAsync(userId);
            if (user == null || !user.IsActive)
                throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        // Valida que la carta exista.
        private async Task ValidateCardExistence(int cardId)
        {
            var card = await _cardDao.GetCardByIdAsync(cardId);
            if (card == null)
                throw new NotFoundException(ErrorMessages.CardNotFound);
        }
    }
}
