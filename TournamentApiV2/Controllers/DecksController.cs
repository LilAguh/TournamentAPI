using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Decks;
using Models.Entities;
using Services.Implementations;
using System.Security.Claims;

namespace TournamentApiV2.Controllers
{
    [Authorize]
    [Route("api/decks")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly DeckService _deckService;

        public DecksController(DeckService deckService)
        {
            _deckService = deckService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeck([FromBody] DeckRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var deck = await _deckService.CreateDeckAsync(dto, userId);
            return CreatedAtAction(nameof(GetDeckById), new { deckId = deck.Id }, deck);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDecks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var decks = await _deckService.GetDecksByUserAsync(userId);
            return Ok(decks);
        }

        [HttpGet("{deckId}")]
        public async Task<IActionResult> GetDeckById(int deckId)
        {
            var deck = await _deckService.GetDeckByIdAsync(deckId);
            if (deck == null)
                return NotFound("Mazo no encontrado");
            return Ok(deck);
        }

        [HttpDelete("{deckId}")]
        public async Task<IActionResult> DeleteDeck(int deckId)
        {
            // Opcional: verificar que el mazo pertenezca al usuario
            var success = await _deckService.DeleteDeckAsync(deckId);
            return success ? Ok("Mazo eliminado") : NotFound("Mazo no encontrado");
        }
    }
}
