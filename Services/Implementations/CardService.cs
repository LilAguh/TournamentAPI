
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Cards;
using Services.Interfaces;

namespace Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardDao _cardDao;

        public CardService(ICardDao cardDao)
        {
            _cardDao = cardDao;
        }

        public async Task<int> CreateCardAsync (CardRequestDto card, int adminId)
        {
            return await _cardDao.AddCardAsync(card, adminId);
        }

        public async Task<IEnumerable<CardResponseDto>> GetAllCardsAsync()
        {
            return await _cardDao.GetAllCardsAsync();
        }

        public async Task<CardResponseDto?> GetCardByIdAsync(int id)
        {
            return await _cardDao.GetCardByIdAsync(id);
        }

        public async Task<bool> UpdateCardAsync(int id, CardRequestDto card, int adminId)
        {
            var existingCard = await _cardDao.GetCardByIdAsync(id);
            if (existingCard == null)
                throw new KeyNotFoundException("Carta no encontrada");

            return await _cardDao.UpdateCardAsync(id, card);
        }

    }
}
