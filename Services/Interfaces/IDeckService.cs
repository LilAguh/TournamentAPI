
using Models.DTOs.Decks;

namespace Services.Interfaces
{
    public interface IDeckService
    {
        Task<DeckResponseDto> CreateDeckAsync(DeckRequestDto dto, int userId);
        Task<IEnumerable<DeckResponseDto>> GetDecksByUserAsync(int userId);
        Task<DeckResponseDto?> GetDeckByIdAsync(int deckId);
        Task<bool> DeleteDeckAsync(int deckId);
    }
}
