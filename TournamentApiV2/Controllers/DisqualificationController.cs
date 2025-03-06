
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Disqualifications;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace TournamentApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisqualificationController : ControllerBase
    {
        private readonly IDisqualificationService _disqualificationService;

        public DisqualificationController(IDisqualificationService disqualificationService)
        {
            _disqualificationService = disqualificationService;
        }

        [Authorize(Roles = "Judge")]
        [HttpPost]
        public async Task<IActionResult> DisqualifyPlayer([FromBody] DisqualificationRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int judgeId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (judgeId == 0)
            {
                throw new UnauthorizedException("Token inválido o falta información del juez.");
            }

            var disqualificationId = await _disqualificationService.DisqualifyPlayerAsync(requestDto, judgeId);
            return Ok(new { DisqualificationId = disqualificationId });
        }
    }
}
