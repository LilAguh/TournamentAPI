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

        // POST /User/Register
        // Registra un nuevo usuario. Si el usuario está autenticado, se usa el ID del token como creador;
        // en caso contrario se realiza un registro público (solo rol Player permitido).
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto dto)
        {
            int? creatorId = null;
            if (User.Identity?.IsAuthenticated == true)
                creatorId = GetUserIdFromToken();

            var user = await _userService.Register(dto, creatorId);
            return Ok(new { user.Alias, user.Role });
        }

        // PUT /User/{id}
        // Actualiza los datos del usuario. El usuario autenticado solo puede actualizar su propia información.
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequestDto dto)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (userIdFromToken != id)
                throw new ForbiddenException(ErrorMessages.IdDiffer);

            var updatedUser = await _userService.UpdateUser(id, dto);
            return Ok(updatedUser);
        }

        // POST /User/ChangePassword
        // Cambia la contraseña del usuario autenticado.
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            var userIdFromToken = GetUserIdFromToken();
            await _userService.ChangePasswordAsync(userIdFromToken, dto);
            return Ok(new { Message = ErrorMessages.PasswordUpdated });
        }

        // DELETE /User/{id}
        // Desactiva (elimina lógicamente) el usuario autenticado.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (userIdFromToken != id)
                throw new ForbiddenException(ErrorMessages.IdDiffer);

            await _userService.DeleteUser(id);
            return Ok(new { Message = ErrorMessages.AccountDeactivated });
        }

        // DELETE /User/DeletePermanent/{id}
        // Elimina permanentemente el usuario autenticado.
        [Authorize]
        [HttpDelete("DeletePermanent/{id}")]
        public async Task<IActionResult> DeleteUserPermanently(int id)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (userIdFromToken != id)
                throw new ForbiddenException(ErrorMessages.IdDiffer);

            await _userService.DeletePermanentUser(id);
            return Ok(new { Message = ErrorMessages.AccountDeleted });
        }

        // GET /User/{id}
        // Obtiene los datos del usuario, solo accesible para Admin.
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }

        // Método auxiliar para extraer el ID del usuario desde el token JWT.
        private int GetUserIdFromToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedException(ErrorMessages.GetUserIdException);
            return int.Parse(userId);
        }
    }
}