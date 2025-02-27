
using DataAccess.DAOs.Interfaces;
using Models.DTOs.CardDecks;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class CardDeckService : ICardDeckService
    {
        private readonly ICardDeckDao _cardDeckDao;
        private readonly ICardDao _cardDao;

        public CardDeckService(ICardDeckDao cardDeckDao, ICardDao cardDao)
        {
            _cardDeckDao = cardDeckDao;
            _cardDao = cardDao;
        }

        public async Task AddCardToDeckAsync(int deckId, int cardId)
        {
            var currentCards = await _cardDeckDao.GetCardsInDeckAsync(deckId);
            if (currentCards.Count() >= 15)
                throw new ValidationException("El mazo ya tiene 15 cartas.");

            if (currentCards.Any(c => c.CardId == cardId))
                throw new ValidationException("La carta ya está en el mazo.");

            var card = await _cardDao.GetCardByIdAsync(cardId);
            if (card == null)
                throw new NotFoundException("Carta no existe");

            await _cardDeckDao.AddCardToDeckAsync(deckId, cardId);
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
