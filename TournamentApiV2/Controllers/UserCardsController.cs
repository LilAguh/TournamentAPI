using Microsoft.AspNetCore.Mvc;

namespace TournamentApiV2.Controllers
{
    public class UserCardsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
