
using DataAccess.DAOs.Interfaces;
using Models.DTOs.CardDecks;
using Services.Interfaces;

namespace Services.Implementations
{
    public class CardDeckService : ICardDeckService
    {
        private readonly ICardDeckDao _cardDeckDao;

        public CardDeckService(ICardDeckDao cardDeckDao)
        {
            _cardDeckDao = cardDeckDao;
        }

        public async Task<bool> AddCardToDeckAsync(int deckId, AddCardDeckRequestDto dto)
        {
            return await _cardDeckDao.AddCardToDeckAsync(deckId, dto.CardId);
        }

        public async Task<bool> RemoveCardFromDeckAsync(int deckId, int cardId)
        {
            return await _cardDeckDao.RemoveCardFromDeckAsync(deckId, cardId);
        }

        public async Task<IEnumerable<CardDeckResponseDto>> GetCardsInDeckAsync(int deckId)
        {
            return await _cardDeckDao.GetCardsInDeckAsync(deckId);
        }
    }
}
