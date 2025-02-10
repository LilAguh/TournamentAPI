﻿using Config;
using DataAccess.DAOs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.User;
using Services.Interfaces;
using System.Security.Claims;


namespace TournamentApiV2.Controllers
{
    [Authorize(Roles = "Admin")] // Solo accesible por Admins
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserDao _userDao;

        public AdminController(IUserService userService, IUserDao userDao)
        {
            _userService = userService;
            _userDao = userDao;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] AdminRegisterRequestDto dto)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.CreateUserByAdmin(dto, adminId);

            return Ok(new
            {
                user.Alias,
                user.Email,
                user.Role
            });
        }
  
    [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }
    }
}