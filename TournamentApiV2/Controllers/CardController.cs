using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard(int id, [FromBody] CardRequestDto cardDto)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var updated = await _cardService.UpdateCardAsync(id, cardDto, adminId);
            return updated ? Ok("Carta actualizada") : NotFound("Carta no encontrada");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var deleted = await _cardService.DeleteCardAsync(id, adminId);
            return deleted ? Ok("Carta eliminada") : NotFound("Carta no encontrada");
        }
    }
}
