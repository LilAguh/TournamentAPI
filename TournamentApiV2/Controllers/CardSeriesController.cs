using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.CardSeries;
using Services.Implementations;

namespace TournamentApiV2.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/cardseries")]
    [ApiController]
    public class CardSeriesController : ControllerBase
    {
        private readonly CardSeriesService _cardSeriesService;

        public CardSeriesController(CardSeriesService cardSeriesService)
        {
            _cardSeriesService = cardSeriesService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCardToSeries([FromBody] AddCardSeriesRequestDto dto)
        {
            var success = await _cardSeriesService.AddCardToSeriesAsync(dto);
            return success ? Ok("Carta asignada a la serie") : BadRequest("Error al asignar la carta, puede que ya exista la relación");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCardFromSeries([FromBody] RemoveCardSeriesRequestDto dto)
        {
            var success = await _cardSeriesService.RemoveCardFromSeriesAsync(dto.CardId, dto.SeriesId);
            return success ? Ok("Carta removida de la serie") : BadRequest("Error al remover la carta");
        }

        // Endpoint para obtener todas las cartas de una serie
        [AllowAnonymous]
        [HttpGet("byseries/{seriesId}")]
        public async Task<IActionResult> GetCardsBySeries(int seriesId)
        {
            var result = await _cardSeriesService.GetCardsBySeriesAsync(seriesId);
            return Ok(result);
        }

        // Endpoint para obtener todas las series a las que pertenece una carta
        [AllowAnonymous]
        [HttpGet("bycard/{cardId}")]
        public async Task<IActionResult> GetSeriesByCard(int cardId)
        {
            var result = await _cardSeriesService.GetSeriesByCardAsync(cardId);
            return Ok(result);
        }
    }
}
