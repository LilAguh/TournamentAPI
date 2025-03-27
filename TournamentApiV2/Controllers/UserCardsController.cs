
using Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.UserCards;
using Services.Interfaces;
using System.Security.Claims;
using static Models.Exceptions.CustomException;

namespace TournamentApiV2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserCardsController : ControllerBase
    {
        private readonly IUserCardService _userCardService;

        public UserCardsController(IUserCardService userCardService)
        {
            _userCardService = userCardService;
        }

        // POST /UserCards
        // Agrega una carta a la colección del usuario autenticado.
        [HttpPost]
        public async Task<IActionResult> AddUserCard([FromBody] AddUserCardRequestDto dto)
        {
            await _userCardService.AddUserCardAsync(GetUserId(), dto);
            return Ok(ErrorMessages.CardAddCollection);
        }

        // GET /UserCards
        // Retorna la lista de cartas que el usuario autenticado posee.
        [HttpGet]
        public async Task<IActionResult> GetUserCards()
        {
            var userCards = await _userCardService.GetUserCardsAsync(GetUserId());
            return Ok(userCards);
        }

        // DELETE /UserCards/{cardId}
        // Elimina una carta de la colección del usuario autenticado.
        [HttpDelete("{cardId}")]
        public async Task<IActionResult> RemoveUserCard(int cardId)
        {
            await _userCardService.RemoveUserCardAsync(GetUserId(), cardId);
            return Ok(ErrorMessages.CardRemovedCollection);
        }

        // Método auxiliar para extraer el ID del usuario desde el token JWT.
        // Lanza una excepción si el ID no se encuentra o no tiene el formato correcto.
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedException(ErrorMessages.AdminTokenNotFound);

            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedException(ErrorMessages.InvalidFormatTokenId);

            return userId;
        }
    }
}