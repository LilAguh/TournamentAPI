

using Config;
using DataAccess.DAOs.Interfaces;
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
    }
}
