﻿using Microsoft.AspNetCore.Mvc;
using Models.DTOs.User;
using Models.Entities;
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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _authService.AuthenticateAsync(dto.Alias, dto.Password);
            var token = _authService.GenerateJwtToken(user);
            return Ok(new
            {
                Token = token,
                User = new { user.Id, user.Alias, user.Email, user.Role }
            });
        }
    }
}