
using Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Decks;
using Services.Interfaces;
using System.Security.Claims;
using static Models.Exceptions.CustomException;

namespace TournamentApiV2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DecksController : ControllerBase
    {
        private readonly IDeckService _deckService;

        public DecksController(IDeckService deckService)
        {
            _deckService = deckService;
        }

        // POST /Decks: Crea un nuevo mazo para el usuario autenticado.
        [HttpPost]
        public async Task<IActionResult> CreateDeck([FromBody] DeckRequestDto dto)
        {
            var deck = await _deckService.CreateDeckAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetDeckById), new { deckId = deck.Id }, deck);
        }

        // GET /Decks: Retorna todos los mazos del usuario autenticado.
        [HttpGet]
        public async Task<IActionResult> GetUserDecks()
        {
            var decks = await _deckService.GetDecksByUserAsync(GetUserId());
            return Ok(decks);
        }

        // GET /Decks/{deckId}: Retorna el mazo específico por su ID.
        [HttpGet("{deckId}")]
        public async Task<IActionResult> GetDeckById(int deckId)
        {
            var deck = await _deckService.GetDeckByIdAsync(deckId);
            return Ok(deck);
        }

        // DELETE /Decks/{deckId}: Elimina el mazo si pertenece al usuario autenticado.
        [HttpDelete("{deckId}")]
        public async Task<IActionResult> DeleteDeck(int deckId)
        {
            await _deckService.DeleteDeckAsync(deckId, GetUserId());
            return Ok(ErrorMessages.DeleteDeck);
        }

        // Método auxiliar para extraer el ID del usuario desde el token JWT.
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedException(ErrorMessages.AdminTokenNotFound);
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedException(ErrorMessages.InvalidFormatTokenId);
            return userId;
        }
    }
}
