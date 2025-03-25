

using Models.DTOs.Country;

namespace DataAccess.DAOs.Interfaces
{
    public interface ICountryDao
    {
        Task<bool> CountryExists(string code);
        Task<IEnumerable<CountryResponseDto>> GetAllCountriesAsync();
    }
}
