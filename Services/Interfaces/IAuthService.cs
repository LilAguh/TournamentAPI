


using Models.Entities;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string emailOrAlias, string password);
        string GenerateJwtToken(User user);
    }
}
