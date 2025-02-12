
using DataAccess.DAOs.Implementations;
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
        public async Task AddUserCardAsync(int userId, AddUserCardRequestDto dto)
        {
            // Validar que el usuario exista
            // No es necesario por el echo de que el usuario debe existir para llegar a esta instancia
            // Por si a caso lo dejamos
            var user = await _userDao.GetUserByIdAsync(userId);
            if (user == null || !user.IsActive)
                throw new NotFoundException("Usuario no encontrado.");

            // Validar que la carta exista
            var card = await _cardDao.GetCardByIdAsync(dto.CardId);
            if (card == null)
                throw new NotFoundException("Carta no encontrada.");

            // Validar cantidad
            if (dto.Quantity <= 0)
                throw new ValidationException("La cantidad debe ser mayor a cero.");

            // Intentar agregar la carta al usuario
            var success = await _userCardDao.AddUserCardAsync(userId, dto);
            if (!success)
                throw new ValidationException("No se pudo agregar la carta al usuario.");
        }
        public async Task<IEnumerable<UserCardResponseDto>> GetUserCardsAsync(int userId)
        {
            var userCards = await _userCardDao.GetUserCardsAsync(userId);
            if (!userCards.Any())
                throw new NotFoundException("No se encontraron cartas en tu colección.");

            return userCards;
        }

        public async Task RemoveUserCardAsync(int userId, int cardId)
        {
            var success = await _userCardDao.RemoveUserCardAsync(userId, cardId);
            if (!success)
                throw new NotFoundException("La carta no existe en tu colección.");
        }
    }
}
