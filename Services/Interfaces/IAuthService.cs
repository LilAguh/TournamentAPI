


using Models.Entities;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string identifier, string password);
        string GenerateJwtToken(User user);
    }
}
