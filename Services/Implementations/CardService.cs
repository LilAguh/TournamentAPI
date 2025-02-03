
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

    }
}
