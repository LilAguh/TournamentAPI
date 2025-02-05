
using Models.DTOs.CardDecks;

namespace DataAccess.DAOs.Interfaces
{
    public interface ICardDeckDao
    {
        Task<bool> AddCardToDeckAsync(int deckId, int cardId);
        Task<bool> RemoveCardFromDeckAsync(int deckId, int cardId);
        Task<int> GetCardCountInDeckAsync(int deckId);
        //Despues ver si la lista de cartas en un mazo con traerlo con mas detalles
        Task<IEnumerable<CardDeckResponseDto>> GetCardsInDeckAsync(int deckId);
    }
}
