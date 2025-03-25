using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace TournamentApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        // GET /Countries
        // Obtiene y retorna la lista de todos los países.
        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            return Ok(countries);
        }
    }
}
