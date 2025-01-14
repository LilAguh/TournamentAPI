using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace TournamentAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController (IUserService userService)
        {
            _userService = userService;
        }
    }
}
