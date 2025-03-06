using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Series;
using Services.Interfaces;

namespace TournamentApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeriesController : ControllerBase
    {
        private readonly ISerieService _seriesService;

        public SeriesController(ISerieService seriesService)
        {
            _seriesService = seriesService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSeries([FromBody] SeriesRequestDto dto)
        {
            var series = await _seriesService.CreateSeriesAsync(dto);
            return CreatedAtAction(nameof(GetSeriesById), new { id = series.Id }, series);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSeries()
        {
            var seriesList = await _seriesService.GetAllSeriesAsync();
            return Ok(seriesList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeriesById(int id)
        {
            var series = await _seriesService.GetSeriesByIdAsync(id);
            return Ok(series);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeries(int id, [FromBody] SeriesRequestDto dto)
        {
            var updatedSeries = await _seriesService.UpdateSeriesAsync(id, dto);
            return Ok(updatedSeries);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeries(int id)
        {
            await _seriesService.DeleteSeriesAsync(id);
            return Ok("Serie eliminada");
        }
    }
}
