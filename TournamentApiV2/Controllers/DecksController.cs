using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Decks;
using Services.Interfaces;
using System.Security.Claims;

namespace TournamentApiV2.Controllers
{
    [Authorize]
    [Route("api/decks")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly IDeckService _deckService;

        public DecksController(IDeckService deckService)
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
            return Ok(deck);
        }

        [HttpDelete("{deckId}")]
        public async Task<IActionResult> DeleteDeck(int deckId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _deckService.DeleteDeckAsync(deckId, userId);
            return Ok("Mazo eliminado");
        }
    }
}
