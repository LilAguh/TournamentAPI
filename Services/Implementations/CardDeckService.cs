
using DataAccess.DAOs.Interfaces;
using Models.DTOs.CardDecks;
using Models.Entities;
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

        public async Task AddCardToDeckAsync(int deckId, AddCardDeckRequestDto dto)
        {
            var currentCards = await _cardDeckDao.GetCardsInDeckAsync(deckId);
            if (currentCards.Any(c => c.CardId == dto.CardId))
                throw new InvalidOperationException("La carta ya está asignada al mazo.");

            if (currentCards.Count() >= 15)
                throw new InvalidOperationException("El mazo ya contiene 15 cartas y no se pueden agregar más.");

            if (currentCards.Any(c => c.CardId == dto.CardId))
                throw new ValidationException("La carta ya está en el mazo.");

            var card = await _cardDao.GetCardByIdAsync(dto.CardId);
            if (card == null)
                throw new NotFoundException("Carta no existe");

            await _cardDeckDao.AddCardToDeckAsync(deckId, dto.CardId);
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
