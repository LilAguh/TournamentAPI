using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Services.Implementations;

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
                return Ok(new { user.Alias, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
