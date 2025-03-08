

namespace Services.Interfaces
{
    public interface ICountryService
    {
        Task ValidateCountryAsync(string countryCode);
    }
}
