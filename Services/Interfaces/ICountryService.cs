

using Models.DTOs.Country;

namespace Services.Interfaces
{
    public interface ICountryService
    {
        Task ValidateCountryAsync(string countryCode);
        Task<IEnumerable<CountryResponseDto>> GetAllCountriesAsync();
    }
}
