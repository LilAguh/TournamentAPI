
using Models.DTOs.Decks;

namespace Services.Interfaces
{
    public interface IDeckDao
    {
        Task<int> CreateDeckAsync(DeckRequestDto dto, int userId);
        Task<DeckResponseDto?> GetDeckByIdAsync(int deckId);
        Task<IEnumerable<DeckResponseDto>> GetDecksByUserAsync(int userId);
        Task<bool> DeleteDeckAsync(int deckId);
        Task<bool> IsDeckOwnedByUser(int deckId, int userId);
    }
}
