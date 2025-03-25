

using Config;
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Country;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly ICountryDao _countryDao;

        public CountryService(ICountryDao countryDao)
        {
            _countryDao = countryDao;
        }

        public async Task ValidateCountryAsync(string countryCode)
        {
            bool countryExists = await _countryDao.CountryExists(countryCode);
            if (!countryExists)
                throw new ValidationException(ErrorMessages.InvalidCountryCode);
        }

        public async Task<IEnumerable<CountryResponseDto>> GetAllCountriesAsync()
        {
            return await _countryDao.GetAllCountriesAsync();
        }
    }
}
