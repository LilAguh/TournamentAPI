
using Models.DTOs.Decks;
using Services.Interfaces;

namespace Services.Implementations
{
    public class DeckService : IDeckService
    {
        private readonly IDeckDao _deckDao;

        public DeckService(IDeckDao deckDao)
        {
            _deckDao = deckDao;
        }

        public async Task<DeckResponseDto> CreateDeckAsync(DeckRequestDto dto, int userId)
        {
            int deckId = await _deckDao.CreateDeckAsync(dto, userId);
            var deck = await _deckDao.GetDeckByIdAsync(deckId);
            if (deck == null)
                throw new Exception("Error al crear el mazo.");
            return deck;
        }
        public async Task<IEnumerable<DeckResponseDto>> GetDecksByUserAsync(int userId)
        {
            return await _deckDao.GetDecksByUserAsync(userId);
        }

        public async Task<DeckResponseDto?> GetDeckByIdAsync(int deckId)
        {
            return await _deckDao.GetDeckByIdAsync(deckId);
        }

        public async Task<bool> DeleteDeckAsync(int deckId)
        {
            return await _deckDao.DeleteDeckAsync(deckId);
        }
    }
}
