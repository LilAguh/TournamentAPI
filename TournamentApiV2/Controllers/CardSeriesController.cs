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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCardToSeries([FromBody] AddCardSeriesRequestDto dto)
        {
            await _cardSeriesService.AddCardToSeriesAsync(dto);
            return Ok("Carta asignada a la serie");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> RemoveCardFromSeries([FromBody] RemoveCardSeriesRequestDto dto)
        {
            await _cardSeriesService.RemoveCardFromSeriesAsync(dto.CardId, dto.SeriesId);
            return Ok("Carta removida de la serie");
        }

        [HttpGet("BySeries/{seriesId}")]
        public async Task<IActionResult> GetCardsBySeries(int seriesId)
        {
            var result = await _cardSeriesService.GetCardsBySeriesAsync(seriesId);
            return Ok(result);
        }

        [HttpGet("ByCard/{cardId}")]
        public async Task<IActionResult> GetSeriesByCard(int cardId)
        {
            var result = await _cardSeriesService.GetSeriesByCardAsync(cardId);
            return Ok(result);
        }
    }
}
