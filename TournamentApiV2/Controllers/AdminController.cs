using Config;
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
        public async Task<IActionResult> CreateUser([FromBody] AdminRegisterDto dto)
        {
            try
            {
                var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var user = await _userService.CreateUserByAdmin(dto, adminId);

                return Ok(new
                {
                    user.Id,
                    user.Alias,
                    user.Email,
                    user.Role
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ErrorMessages.InternalServerError });
            }
        }
  
    [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { Error = ErrorMessages.UserNotFound });

            return Ok(user);
        }
    }
}