using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace TournamentApiV2.Controllers
{
    [Route("api/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [Authorize(Roles = "Admin,Organizer")]
        [HttpPost("first-round/{tournamentId}")]
        public async Task<IActionResult> GenerateFirstRoundMatches(int tournamentId, [FromBody] List<int> playerIds)
        {
            if (playerIds == null || playerIds.Count % 2 != 0)
                return BadRequest("La cantidad de jugadores debe ser par");

            await _matchService.CreateFirstRoundMatchesAsync(tournamentId, playerIds);
            return Ok("Partidos de la primera ronda generados exitosamente");
        }
    }
}
