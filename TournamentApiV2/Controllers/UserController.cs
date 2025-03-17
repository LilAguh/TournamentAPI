using Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.User;
using Services.Interfaces;
using static Models.Exceptions.CustomException;
using System.Security.Claims;

namespace TournamentApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto dto)
        {
            int? creatorId = null;

            // Solo intenta obtener el ID si el token es válido y el usuario está autenticado
            if (User.Identity?.IsAuthenticated == true)
            {
                creatorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            try
            {
                var user = await _userService.Register(dto, creatorId);
                return Ok(new { user.Alias, user.Role });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, ex.Message); // 403 si no tiene permisos
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message); // 400 si falta el rol para Admin
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequestDto dto)
        {
            var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userIdFromToken != id)
                throw new ForbiddenException(ErrorMessages.IdDiffer);

            var updatedUser = await _userService.UpdateUser(id, dto);
            return Ok(updatedUser);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            await _userService.ChangePasswordAsync(userIdFromToken, dto);
            return Ok(new { Message = "Contraseña actualizada correctamente." });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userIdFromToken != id)
                throw new ForbiddenException(ErrorMessages.IdDiffer);

            await _userService.DeleteUser(id);
            return Ok(new { Message = ErrorMessages.AccountDeactivated });
        }

        [HttpDelete("DeletePermanent/{id}")]
        public async Task<IActionResult> DeleteUserPermanently(int id)
        {
            await _userService.DeletePermanentUser(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }
    }
}