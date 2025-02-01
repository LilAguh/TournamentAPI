using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.Entities;
using Services.Implementations;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

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
            var user = await _authService.AuthenticateAsync(dto.EmailOrAlias, dto.Password);
            var token = _authService.GenerateJwtToken(user);
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
    }
}