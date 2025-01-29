using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;

namespace TournamentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                // Autenticar usuario
                var user = await _authService.AuthenticateAsync(dto.EmailOrAlias, dto.Password);

                // Generar token
                var token = _authService.GenerateJwtToken(user);

                // Responder con datos relevantes
                return Ok(new
                {
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.Alias,
                        user.Email,
                        user.Role
                    }
                });
            }
            catch (ArgumentException ex)
            {
                // Credenciales inválidas
                return Unauthorized(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                // Error interno del servidor
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}