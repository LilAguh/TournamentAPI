
using DataAccess.Database;
using Models.DTOs.Decks;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

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
                throw new ValidationException("Error al crear el mazo.");

            return deck;
        }

        public async Task<IEnumerable<DeckResponseDto>> GetDecksByUserAsync(int userId)
        {
            var decks = await _deckDao.GetDecksByUserAsync(userId);
            if (!decks.Any())
                throw new NotFoundException("No tienes mazos registrados.");

            return decks;
        }

        public async Task<DeckResponseDto> GetDeckByIdAsync(int deckId)
        {
            var deck = await _deckDao.GetDeckByIdAsync(deckId);
            if (deck == null)
                throw new NotFoundException("Mazo no encontrado.");

            return deck;
        }

        public async Task DeleteDeckAsync(int deckId, int userId)
        {
            var deck = await _deckDao.GetDeckByIdAsync(deckId);
            if (deck == null || deck.UserId != userId)
                throw new ForbiddenException("No puedes eliminar este mazo.");

            await _deckDao.DeleteDeckAsync(deckId);
        }
    }
}
