
using DataAccess.DAOs.Interfaces;
using Models.DTOs.CardDecks;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class CardDeckService : ICardDeckService
    {
        private readonly ICardDeckDao _cardDeckDao;

        public CardDeckService(ICardDeckDao cardDeckDao)
        {
            _cardDeckDao = cardDeckDao;
        }

        public async Task AddCardToDeckAsync(int deckId, int cardId)
        {
            var currentCount = await _cardDeckDao.GetCardCountInDeckAsync(deckId);
            if (currentCount >= 15)
                throw new ValidationException("El mazo ya tiene 15 cartas.");

            var success = await _cardDeckDao.AddCardToDeckAsync(deckId, cardId);
            if (!success)
                throw new ValidationException("La carta ya está en el mazo.");
        }

        public async Task RemoveCardFromDeckAsync(int deckId, int cardId)
        {
            var success = await _cardDeckDao.RemoveCardFromDeckAsync(deckId, cardId);
            if (!success)
                throw new NotFoundException("La carta no está en el mazo.");
        }

        public async Task<IEnumerable<CardDeckResponseDto>> GetCardsInDeckAsync(int deckId)
        {
            var cards = await _cardDeckDao.GetCardsInDeckAsync(deckId);
            if (!cards.Any())
                throw new NotFoundException("El mazo está vacío.");

            return cards;
        }
    }
}
