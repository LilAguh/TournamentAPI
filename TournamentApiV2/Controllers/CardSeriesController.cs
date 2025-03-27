
using Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.CardSeries;
using Services.Interfaces;

namespace TournamentApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardSeriesController : ControllerBase
    {
        private readonly ICardSeriesService _cardSeriesService;

        public CardSeriesController(ICardSeriesService cardSeriesService)
        {
            _cardSeriesService = cardSeriesService;
        }

        // POST /CardSeries
        // Agrega una carta a una serie. Solo accesible para administradores.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCardToSeries([FromBody] CardSeriesRequestDto dto)
        {
            await _cardSeriesService.AddCardToSeriesAsync(dto);
            return Ok(new { Message = ErrorMessages.CardAddSeries });
        }

        // DELETE /CardSeries
        // Elimina una carta de una serie. Solo accesible para administradores.
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> RemoveCardFromSeries([FromBody] CardSeriesRequestDto dto)
        {
            await _cardSeriesService.RemoveCardFromSeriesAsync(dto.CardId, dto.SeriesId);
            return Ok(new { Message = ErrorMessages.CardRemoveSeries });
        }

        // GET /CardSeries/BySeries/{seriesId}
        // Obtiene todas las cartas pertenecientes a una serie.
        [HttpGet("BySeries/{seriesId}")]
        public async Task<IActionResult> GetCardsBySeries(int seriesId)
        {
            var result = await _cardSeriesService.GetCardsBySeriesAsync(seriesId);
            return Ok(result);
        }

        // GET /CardSeries/ByCard/{cardId}
        // Obtiene todas las series a las que pertenece una carta.
        [HttpGet("ByCard/{cardId}")]
        public async Task<IActionResult> GetSeriesByCard(int cardId)
        {
            var result = await _cardSeriesService.GetSeriesByCardAsync(cardId);
            return Ok(result);
        }
    }
}
