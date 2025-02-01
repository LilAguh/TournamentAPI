using Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services.Interfaces;
using System.Security.Claims;
using static Models.Exceptions.CustomException;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register(PlayerRegisterDto dto)
        {
            var user = await _userService.Register(dto);
            return Ok(new
            {
                user.Id,
                user.Alias,
                user.Email
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userIdFromToken != id)
                throw new ForbiddenException(ErrorMessages.IdDiffer);

            var updatedUser = await _userService.UpdateUser(id, dto);
            return Ok(updatedUser);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
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
    }
}
