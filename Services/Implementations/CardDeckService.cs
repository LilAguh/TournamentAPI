
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
            // Obtener las cartas actuales en el mazo
            var currentCards = await _cardDeckDao.GetCardsInDeckAsync(deckId);

            // Validar que el total (actual + nuevas) no exceda el límite de 15 cartas
            if (currentCards.Count() + dto.CardId.Count > 15)
                throw new InvalidOperationException("El mazo ya contiene 15 cartas o se supera el límite al agregar las nuevas cartas.");

            // Validar para cada carta en la solicitud
            foreach (var cardId in dto.CardId)
            {
                // Verificar que la carta no se encuentre ya en el mazo
                if (currentCards.Any(c => c.CardId == cardId))
                    throw new ValidationException($"La carta con ID {cardId} ya está asignada al mazo.");

                // Verificar que la carta exista
                var card = await _cardDao.GetCardByIdAsync(cardId);
                if (card == null)
                    throw new NotFoundException($"La carta con ID {cardId} no existe.");
            }

            // Agregar cada carta al mazo
            foreach (var cardId in dto.CardId)
            {
                await _cardDeckDao.AddCardToDeckAsync(deckId, cardId);
            }
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
