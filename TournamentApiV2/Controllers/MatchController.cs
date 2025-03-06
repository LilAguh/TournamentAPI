using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Matches;
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

        //[Authorize(Roles = "Admin,Organizer")]
        [HttpPost("generate-round/{tournamentId}")]
        public async Task<IActionResult> GenerateRoundMatches(int tournamentId)
        {

            await _matchService.CreateRoundMatchAsync(tournamentId);
            return Ok("Partidos de la ronda generados correctamente");
        }

        //[Authorize(Roles = "Judge")]
        [HttpPost("result")]
        public async Task<IActionResult> SubmitMatchResult([FromBody] MatchResultRequestDto dto)
        {
            bool success = await _matchService.UpdateMatchWinnerAsync(dto.MatchId, dto.WinnerId);
            if (success)
            {
                return Ok("Resultado registrado exitosamente");
            }
            return NotFound("Partido no encontrado o error al actualizar");
        }
    }
}
