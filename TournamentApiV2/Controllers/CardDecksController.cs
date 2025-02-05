using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.CardDecks;
using Services.Implementations;

namespace TournamentApiV2.Controllers
{
    [Authorize]
    [Route("api/decks/{deckId}/cards")]
    [ApiController]
    public class CardDecksController : ControllerBase
    {
        private readonly CardDeckService _cardDeckService;

        public CardDecksController(CardDeckService cardDeckService)
        {
            _cardDeckService = cardDeckService;
        }

        // Agregar una carta al mazo
        [HttpPost]
        public async Task<IActionResult> AddCardToDeck(int deckId, [FromBody] AddCardDeckRequestDto dto)
        {
            var success = await _cardDeckService.AddCardToDeckAsync(deckId, dto);
            return success ? Ok("Carta agregada al mazo") : BadRequest("No se pudo agregar la carta (posiblemente ya existe o se alcanzó el límite de 15)");
        }

        // Obtener las cartas del mazo
        [HttpGet]
        public async Task<IActionResult> GetCardsInDeck(int deckId)
        {
            var cards = await _cardDeckService.GetCardsInDeckAsync(deckId);
            return Ok(cards);
        }

        // Remover una carta del mazo
        [HttpDelete("{cardId}")]
        public async Task<IActionResult> RemoveCardFromDeck(int deckId, int cardId)
        {
            var success = await _cardDeckService.RemoveCardFromDeckAsync(deckId, cardId);
            return success ? Ok("Carta removida del mazo") : BadRequest("No se pudo remover la carta");
        }
    }
}
