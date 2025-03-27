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

        [Authorize(Roles = "Organizer")]
        [HttpPost("GenerateRound/{tournamentId}")]
        public async Task<IActionResult> GenerateRoundMatches(int tournamentId)
        {
            await _matchService.CreateRoundMatchAsync(tournamentId);
            return Ok(new
            {
                Message = "Ronda generada exitosamente",
                TournamentId = tournamentId
            });
        }

        [Authorize(Roles = "Judge")]
        [HttpPost("Result")]
        public async Task<IActionResult> SubmitMatchResult([FromBody] MatchResultRequestDto dto)
        {
            var result = await _matchService.UpdateMatchWinnerAsync(dto);

            return result ?
                Ok(new { Message = "Resultado registrado exitosamente" }) :
                BadRequest(new { Message = "No se pudo actualizar el resultado" });
        }

        [HttpGet("Tournament/{tournamentId}")]
        public async Task<IActionResult> GetTournamentMatches(int tournamentId)
        {
            var matches = await _matchService.GetMatchesByTournamentAsync(tournamentId);
            return Ok(new
            {
                TournamentId = tournamentId,
                Matches = matches
            });
        }
    }
}
