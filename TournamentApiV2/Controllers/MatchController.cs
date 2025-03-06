using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Matches;
using Services.Interfaces;

namespace TournamentApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        //[Authorize(Roles = "Admin,Organizer")]
        [HttpPost("GenerateRound/{tournamentId}")]
        public async Task<IActionResult> GenerateRoundMatches(int tournamentId)
        {

            await _matchService.CreateRoundMatchAsync(tournamentId);
            return Ok("Partidos de la ronda generados correctamente");
        }

        //[Authorize(Roles = "Judge")]
        [HttpPost("Result")]
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
