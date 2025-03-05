using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Matches;
using Models.DTOs.Tournament;
using Models.DTOs.TournamentPlayers;
using Services.Interfaces;
using System.Security.Claims;
using static Models.Exceptions.CustomException;

namespace TournamentApiV2.Controllers
{
    [Route("api/tournaments")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly IMatchService _matchService;

        public TournamentController(ITournamentService tournamentService, IMatchService matchService)
        {
            _tournamentService = tournamentService;
            _matchService = matchService;
        }


        [Authorize(Roles = "Admin, Organizer")]
        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] TournamentRequestDto dto)
        {

            int organizerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var tournament = await _tournamentService.CreateTournamentAsync(dto, organizerId);
            return CreatedAtAction(nameof(GetTournamentById), new { id = tournament.Id }, tournament);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournamentById(int id)
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id);
            if (tournament == null)
                return NotFound("Torneo no encontrado");
            return Ok(tournament);
        }

        [HttpPost("{tournamentId}/register")]
        public async Task<IActionResult> RegisterPlayer(int tournamentId, [FromBody] TournamentPlayerRegistrationDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new ValidationException("No se encontró el identificador del usuario en el token");

            int userId = int.Parse(userIdClaim.Value);

            await _tournamentService.RegisterPlayerAsync(tournamentId, userId, dto.DeckId);
            return Ok("Inscripción exitosa");
        }

        [HttpGet("{tournamentId}/matches")]
        public async Task<IActionResult> GetTournamentMatches(int tournamentId)
        {
            IEnumerable<MatchResponseDto> matches = await _matchService.GetMatchesByTournamentAsync(tournamentId);
            return Ok(matches);
        }

        //[Authorize(Roles = "Organizer,Admin")]
        [HttpPost("{tournamentId}/series/{seriesId}")]
        public async Task<IActionResult> AddAllowedSeries(int tournamentId, int seriesId)
        {
            await _tournamentService.AddAllowedSeriesAsync(tournamentId, seriesId);
            return Ok();
        }

        //[Authorize(Roles = "Organizer,Admin")]
        [HttpDelete("{tournamentId}/series/{seriesId}")]
        public async Task<IActionResult> RemoveAllowedSeries(int tournamentId, int seriesId)
        {
            await _tournamentService.RemoveAllowedSeriesAsync(tournamentId, seriesId);
            return Ok();
        }

        [HttpGet("{tournamentId}/series")]
        public async Task<IActionResult> GetAllowedSeries(int tournamentId)
        {
            var seriesIds = await _tournamentService.GetAllowedSeriesAsync(tournamentId);
            return Ok(seriesIds);
        }

    }
}
