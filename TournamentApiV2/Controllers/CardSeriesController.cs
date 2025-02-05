using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TournamentApiV2.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/cardseries")]
    [ApiController]
    public class CardSeriesController : ControllerBase
    {
    }
}
