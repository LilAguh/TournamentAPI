using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.CardDecks;
using Services.Interfaces;

namespace TournamentApiV2.Controllers
{
    [Authorize]
    [Route("api/decks/{deckId}/cards")]
    [ApiController]
    public class CardDecksController : ControllerBase
    {
        private readonly ICardDeckService _cardDeckService;

        public CardDecksController(ICardDeckService cardDeckService)
        {
            _cardDeckService = cardDeckService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCardToDeck(int deckId, [FromBody] AddCardDeckRequestDto requestDto)
        {
            await _cardDeckService.AddCardToDeckAsync(deckId, requestDto);
            return Ok(new { message = "Cartas agregadas al mazo" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCardsInDeck(int deckId)
        {
            var cards = await _cardDeckService.GetCardsInDeckAsync(deckId);
            return Ok(cards);
        }

        [HttpDelete("{cardId}")]
        public async Task<IActionResult> RemoveCardFromDeck(int deckId, int cardId)
        {
            await _cardDeckService.RemoveCardFromDeckAsync(deckId, cardId);
            return Ok("Carta removida del mazo");
        }
    }
}
