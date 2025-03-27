
using Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.CardDecks;
using Services.Interfaces;

namespace TournamentApiV2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/{deckId}/Cards")]
    public class CardDecksController : ControllerBase
    {
        private readonly ICardDeckService _cardDeckService;

        public CardDecksController(ICardDeckService cardDeckService)
        {
            _cardDeckService = cardDeckService;
        }

        // POST /CardDecks/{deckId}/Cards
        // Agrega cartas al mazo especificado usando los datos del DTO.
        [HttpPost]
        public async Task<IActionResult> AddCardToDeck(int deckId, [FromBody] AddCardDeckRequestDto requestDto)
        {
            await _cardDeckService.AddCardToDeckAsync(deckId, requestDto);
            return Ok(new { message = ErrorMessages.CardsAddDeck });
        }

        // GET /CardDecks/{deckId}/Cards
        // Retorna la lista de cartas asociadas al mazo indicado.
        [HttpGet]
        public async Task<IActionResult> GetCardsInDeck(int deckId)
        {
            var cards = await _cardDeckService.GetCardsInDeckAsync(deckId);
            return Ok(cards);
        }

        // DELETE /CardDecks/{deckId}/Cards/{cardId}
        // Elimina una carta del mazo especificado.
        [HttpDelete("{cardId}")]
        public async Task<IActionResult> RemoveCardFromDeck(int deckId, int cardId)
        {
            await _cardDeckService.RemoveCardFromDeckAsync(deckId, cardId);
            return Ok(ErrorMessages.CardRemovedDeck);
        }
    }
}
