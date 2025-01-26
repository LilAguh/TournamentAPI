using DataAccess.DAOs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services.Implementations;
using System.Security.Claims;


namespace TournamentApiV2.Controllers
{
    [Authorize(Roles = "Admin")] // Solo accesible por Admins
    [ApiController]
    [Route("admin/users")]
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
        public async Task<IActionResult> CreateUser([FromBody] AdminRegisterDto dto)
        {
            // Obtener el ID del admin autenticado desde el token
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Crear el usuario
            var user = await _userService.CreateUserByAdmin(dto, adminId);

            return Ok(new
            {
                user.Id,
                user.Alias,
                user.Email,
                user.Role
            });
        }
  
    [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { Error = "Usuario no encontrado" });

            return Ok(user);
        }
    }
}