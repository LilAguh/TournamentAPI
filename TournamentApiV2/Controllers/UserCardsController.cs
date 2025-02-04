using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.UserCards;
using Services.Implementations;
using System.Security.Claims;

namespace TournamentApiV2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserCardsController : ControllerBase
    {
        private readonly UserCardService _userCardService;

        public UserCardsController(UserCardService userCardService)
        {
            _userCardService = userCardService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUserCard([FromBody] AddUserCardRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await _userCardService.AddUserCardAsync(userId, dto);
            return success ? Ok("Carta acreditada exitosamente") : BadRequest("Error al acreditar la carta");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCards()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userCards = await _userCardService.GetUserCardsAsync(userId);
            return Ok(userCards);
        }

        [HttpDelete("{cardId}")]
        public async Task<IActionResult> RemoveUserCard(int cardId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await _userCardService.RemoveUserCardAsync(userId, cardId);
            return success ? Ok("Carta removida exitosamente") : NotFound("Carta no encontrada en el registro del usuario");
        }
    }
}
