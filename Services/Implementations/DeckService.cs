
using Config;
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

        // Crea un nuevo mazo para el usuario y retorna el mazo creado.
        // Llama al DAO para crear el mazo y luego obtiene el mazo por su ID.
        // Lanza una ValidationException si el mazo no se creó correctamente.
        public async Task<DeckResponseDto> CreateDeckAsync(DeckRequestDto dto, int userId)
        {
            int deckId = await _deckDao.CreateDeckAsync(dto, userId);
            var deck = await _deckDao.GetDeckByIdAsync(deckId);
            if (deck == null)
                throw new ValidationException(ErrorMessages.ErrorCreateDeck);

            return deck;
        }

        // Retorna todos los mazos asociados a un usuario.
        // Lanza una NotFoundException si el usuario no tiene mazos registrados.
        public async Task<IEnumerable<DeckResponseDto>> GetDecksByUserAsync(int userId)
        {
            var decks = await _deckDao.GetDecksByUserAsync(userId);
            if (!decks.Any())
                throw new NotFoundException(ErrorMessages.NoRegisteredDecks);

            return decks;
        }

        // Retorna un mazo específico por su ID.
        // Lanza una NotFoundException si no se encuentra el mazo.
        public async Task<DeckResponseDto> GetDeckByIdAsync(int deckId)
        {
            var deck = await _deckDao.GetDeckByIdAsync(deckId);
            if (deck == null)
                throw new NotFoundException(ErrorMessages.NotFoundDeck);

            return deck;
        }

        // Elimina un mazo, validando que el usuario es el propietario del mazo.
        // Lanza una ForbiddenException si el mazo no existe o no pertenece al usuario.
        public async Task DeleteDeckAsync(int deckId, int userId)
        {
            var deck = await _deckDao.GetDeckByIdAsync(deckId);
            if (deck == null || deck.UserId != userId)
                throw new ForbiddenException(ErrorMessages.ErrorDeleteDeck);

            await _deckDao.DeleteDeckAsync(deckId);
        }
    }
}
