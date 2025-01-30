using Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services.Implementations;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(PlayerRegisterDto dto)
        {
            try
            {
                var user = await _userService.Register(dto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [Authorize] // Requiere autenticación
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                // Obtener el ID del usuario autenticado desde el token
                var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Verificar que el usuario autenticado solo pueda modificar su propia cuenta
                if (userIdFromToken != id)
                    return Forbid(ErrorMessages.IdDiffer);

                var updatedUser = await _userService.UpdateUser(id, dto);
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [Authorize] // Requiere autenticación
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Obtener el ID del usuario autenticado desde el token
                var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Verificar que el usuario autenticado solo pueda desactivar su propia cuenta
                if (userIdFromToken != id)
                    return Forbid(ErrorMessages.IdDiffer);

                await _userService.DeleteUser(id);

                return Ok(new { Message = ErrorMessages.AccountDeactivated});
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
