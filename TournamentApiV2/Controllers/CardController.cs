using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Cards;
using Services.Implementations;
using System.Security.Claims;

namespace TournamentApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
        private readonly CardService _cardService;

        public CardController(CardService cardService)
        {
            _cardService = cardService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CardRequestDto card)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cardId = await _cardService.CreateCardAsync(card, 21);
            return CreatedAtAction(nameof(GetCardById), new { id = cardId }, new { id = cardId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardById(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null)
                return NotFound("Carta no encontrada");

            return Ok(card);
        }
    }
}
