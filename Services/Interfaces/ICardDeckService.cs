
using Models.DTOs.CardDecks;

namespace Services.Interfaces
{
    public interface ICardDeckService
    {
        Task<bool> AddCardToDeckAsync(int deckId, AddCardDeckRequestDto dto);
        Task<bool> RemoveCardFromDeckAsync(int deckId, int cardId);
        Task<IEnumerable<CardDeckResponseDto>> GetCardsInDeckAsync(int deckId);
    }
}
