using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Tournament;
using Models.Entities;
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

        public TournamentController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] TournamentRequestDto dto)
        {
            // Extraer el OrganizerID del token
            

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

    }
}
