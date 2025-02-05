using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TournamentApiV2.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/series")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
    }
}
