
using Config;
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Cards;
using Models.Enums;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardDao _cardDao;
        private readonly IUserDao _userDao;

        public CardService(ICardDao cardDao, IUserDao userDao)
        {
            _cardDao = cardDao;
            _userDao = userDao;
        }

        // Crea una carta en el sistema, solo si el usuario (admin) es válido.
        public async Task<int> CreateCardAsync(CardRequestDto card, int adminId)
        {
            await ValidateAdminAsync(adminId);
            return await _cardDao.AddCardAsync(card, adminId);
        }

        // Retorna todas las cartas registradas.
        public async Task<IEnumerable<CardResponseDto>> GetAllCardsAsync()
        {
            var cards = await _cardDao.GetAllCardsAsync();
            return cards.Any() ? cards : throw new NotFoundException(ErrorMessages.NoCardsRegistered);
        }

        // Retorna una carta específica según su ID.
        public async Task<CardResponseDto?> GetCardByIdAsync(int id)
        {
            var card = await _cardDao.GetCardByIdAsync(id);
            return card ?? throw new NotFoundException(ErrorMessages.CardNotFound);
            
        }

        // Actualiza una carta existente, validando primero que el admin es válido y la carta existe.
        public async Task<bool> UpdateCardAsync(int id, CardRequestDto card, int adminId)
        {
            await ValidateAdminAsync(adminId);
            await ExistingCardAsync(id);
            return await _cardDao.UpdateCardAsync(id, card);
        }

        // Elimina una carta (eliminación lógica o física, según la implementación), validando que el admin es válido y la carta existe.
        public async Task<bool> DeleteCardAsync(int id, int adminId)
        {
            await ValidateAdminAsync(adminId);
            await ExistingCardAsync(id);
            return await _cardDao.DeleteCardAsync(id);
        }

        // Métodos privados //

        // Método privado para verificar que la carta existe.
        private async Task ExistingCardAsync(int id)
        {
            var existingCard = await _cardDao.GetCardByIdAsync(id);
            if (existingCard == null)
                throw new NotFoundException(ErrorMessages.CardNotFound);
        }

        // Método privado para validar que el usuario con adminId sea un administrador válido.
        private async Task ValidateAdminAsync(int adminId)
        {
            var user = await _userDao.GetUserByIdAsync(adminId);
            if (user == null || user.Role != RoleEnum.Admin)
                throw new ValidationException(ErrorMessages.AdminInvalid);
        }
    }
}
