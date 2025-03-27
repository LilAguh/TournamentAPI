

using Config;
using DataAccess.DAOs.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Models.DTOs.Country;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly ICountryDao _countryDao;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "CountriesCache";

        public CountryService(ICountryDao countryDao, IMemoryCache cache)
        {
            _countryDao = countryDao;
            _cache = cache;
        }

        // Valida que exista un país con el código especificado.
        // Si el país no existe, lanza una excepción de validación.
        public async Task ValidateCountryAsync(string countryCode)
        {
            bool countryExists = await _countryDao.CountryExists(countryCode);
            if (!countryExists)
                throw new ValidationException(ErrorMessages.InvalidCountryCode);
        }

        // Obtiene todos los países registrados.
        // Primero intenta obtenerlos del cache; si no están en el cache, los consulta en la base de datos,
        // y luego los almacena en el cache por 24 horas.
        // Si no se encuentran países, lanza una excepción NotFound.
        public async Task<IEnumerable<CountryResponseDto>> GetAllCountriesAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<CountryResponseDto> countries))
            {
                countries = await _countryDao.GetAllCountriesAsync();
                _cache.Set(CacheKey, countries, TimeSpan.FromHours(24));
            }

            if (countries == null || !countries.Any())
                throw new NotFoundException(ErrorMessages.NoCountriesRegistered);

            return countries;
        }
    }
}
