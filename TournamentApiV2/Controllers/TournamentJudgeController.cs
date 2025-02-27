using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.TournamentJudges;
using Services.Interfaces;

namespace TournamentApiV2.Controllers
{
    [Authorize(Roles = "Organizer, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentJudgeController : ControllerBase
    {
        private readonly ITournamentJudgeService _tournamentJudgeService;

        public TournamentJudgeController(ITournamentJudgeService tournamentJudgeService)
        {
            _tournamentJudgeService = tournamentJudgeService;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignJudge([FromBody] TournamentJudgeRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _tournamentJudgeService.AssignJudgeAsync(requestDto);
            return Ok(new { message = "Juez asignado al torneo" });
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveJudge([FromQuery] int tournamentId, [FromQuery] int judgeId)
        {
            await _tournamentJudgeService.RemoveJudgeAsync(tournamentId, judgeId);
            return Ok(new { message = "Juez removido del torneo" });
        }
    }
}
