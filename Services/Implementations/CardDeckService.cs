
using Config;
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

        // Agrega cartas a un mazo. Valida que el total de cartas (actuales + nuevas) no exceda 15, 
        // que no se agreguen cartas duplicadas y que cada carta exista.
        public async Task AddCardToDeckAsync(int deckId, AddCardDeckRequestDto dto)
        {
            var currentCards = await _cardDeckDao.GetCardsInDeckAsync(deckId);

            if (currentCards.Count() + dto.CardId.Count > 15)
                throw new InvalidOperationException(ErrorMessages.LimitCardExceeded);

            foreach (var cardId in dto.CardId)
            {
                if (currentCards.Any(c => c.CardId == cardId))
                    throw new ValidationException($"The card with ID {cardId} is already assigned to the deck.");

                var card = await _cardDao.GetCardByIdAsync(cardId);
                if (card == null)
                    throw new NotFoundException($"Letter with ID {cardId} does not exist.");
            }

            foreach (var cardId in dto.CardId)
            {
                await _cardDeckDao.AddCardToDeckAsync(deckId, cardId);
            }
        }

        // Elimina una carta de un mazo. Lanza una excepción si la carta no se encuentra en el mazo.
        public async Task RemoveCardFromDeckAsync(int deckId, int cardId)
        {
            var success = await _cardDeckDao.RemoveCardFromDeckAsync(deckId, cardId);
            if (!success)
                throw new NotFoundException(ErrorMessages.CardNotInDeck);
        }

        // Retorna la lista de cartas asociadas a un mazo. Lanza una excepción si el mazo está vacío.
        public async Task<IEnumerable<CardDeckResponseDto>> GetCardsInDeckAsync(int deckId)
        {
            var cards = await _cardDeckDao.GetCardsInDeckAsync(deckId);
            if (!cards.Any())
                throw new NotFoundException(ErrorMessages.DeckEmpty);
            return cards;
        }
    }
}
