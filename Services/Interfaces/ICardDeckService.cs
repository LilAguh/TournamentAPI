
using Models.DTOs.CardDecks;

namespace Services.Interfaces
{
    public interface ICardDeckService
    {
        Task AddCardToDeckAsync(int deckId, int cardId);
        Task RemoveCardFromDeckAsync(int deckId, int cardId);
        Task<IEnumerable<CardDeckResponseDto>> GetCardsInDeckAsync(int deckId);
    }
}
