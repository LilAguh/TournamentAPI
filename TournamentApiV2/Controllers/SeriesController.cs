
using Config;
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

        // POST /Series
        // Crea una nueva serie. Solo los administradores pueden realizar esta acción.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSeries([FromBody] SeriesRequestDto dto)
        {
            var series = await _seriesService.CreateSeriesAsync(dto);
            return CreatedAtAction(nameof(GetSeriesById), new { id = series.Id }, series);
        }

        // GET /Series
        // Obtiene la lista de todas las series registradas.
        [HttpGet]
        public async Task<IActionResult> GetAllSeries()
        {
            var seriesList = await _seriesService.GetAllSeriesAsync();
            return Ok(seriesList);
        }

        // GET /Series/{id}
        // Obtiene una serie específica según su ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeriesById(int id)
        {
            var series = await _seriesService.GetSeriesByIdAsync(id);
            return Ok(series);
        }

        // PUT /Series/{id}
        // Actualiza los datos de una serie existente. Solo los administradores pueden modificar una serie.
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeries(int id, [FromBody] SeriesRequestDto dto)
        {
            var updatedSeries = await _seriesService.UpdateSeriesAsync(id, dto);
            return Ok(updatedSeries);
        }

        // DELETE /Series/{id}
        // Elimina una serie según su ID. Solo los administradores pueden realizar esta acción.
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeries(int id)
        {
            await _seriesService.DeleteSeriesAsync(id);
            return Ok(ErrorMessages.SeriesDeleted);
        }
    }
}
