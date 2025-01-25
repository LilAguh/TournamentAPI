

namespace DataAccess.DAOs.Interfaces
{
    public interface ICountryDao
    {
        Task<bool> CountryExists(string code);
    }
}
