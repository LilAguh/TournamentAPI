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
    public class NewUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public NewUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterRequestDto dto)
        {
            //por el momento voy a implementar asi pero el token se va a revisar en el service
            //vamos a ver como funciona esto
            var user = await _userService.Register(dto);
            return Ok(new { user.Alias, user.Email, user.Role });
        }

        [Authorize(Roles = "Admin, Organizer")]
        [HttpPost("Register/Roles")]
        public async Task<IActionResult> RegisterUserByUser([FromBody] UserRegisterRequestDto dto)
        {
            //Es lo que se me ocurre por el momento, como para llegar a esta fase, investigar el tema del token
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.CreateUserByAdmin(dto, adminId);
            return Ok(new { user.Alias, user.Email, user.Role });
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