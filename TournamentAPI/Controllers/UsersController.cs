using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.Exceptions;
using Services.Interfaces;

namespace TournamentAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController (IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                var userResponse = await _userService.Register(userDto);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register/player")]
        public async Task<IActionResult> RegisterPlayer([FromBody] PlayerRegisterDto playerRegisterDto)
        {
            try
            {
                var userResponse = await _userService.RegisterPlayer(playerRegisterDto);
                return Ok(userResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminsRegisterDto adminsRegisterDto)
        {
            try
            {
                var createdById = int.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException("User ID not found."));
                var userResponse = await _userService.RegisterAdmin(adminsRegisterDto, createdById);
                return Ok(userResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                var token = await _userService.Login(userLoginDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var userResponse = await _userService.GetUserById(id);
            return Ok(userResponse);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDto userDto)
        {
            await _userService.UpdateUser(id, userDto);
            return NoContent();
        }

        [Authorize]
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            await _userService.DeactivateUser(id);
            return NoContent();
        }
    }
}
