using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Series;
using Services.Implementations;

namespace TournamentApiV2.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/series")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly SerieService _seriesService;

        public SeriesController(SerieService seriesService)
        {
            _seriesService = seriesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSeries([FromBody] SeriesRequestDto dto)
        {
            var series = await _seriesService.CreateSeriesAsync(dto);
            return CreatedAtAction(nameof(GetSeriesById), new { id = series.Id }, series);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllSeries()
        {
            var seriesList = await _seriesService.GetAllSeriesAsync();
            return Ok(seriesList);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeriesById(int id)
        {
            var series = await _seriesService.GetSeriesByIdAsync(id);
            if (series == null)
                return NotFound("Serie no encontrada");
            return Ok(series);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeries(int id, [FromBody] SeriesRequestDto dto)
        {
            var updatedSeries = await _seriesService.UpdateSeriesAsync(id, dto);
            return updatedSeries != null ? Ok(updatedSeries) : NotFound("Serie no encontrada");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeries(int id)
        {
            var success = await _seriesService.DeleteSeriesAsync(id);
            return success ? Ok("Serie eliminada") : NotFound("Serie no encontrada");
        }
    }
}
