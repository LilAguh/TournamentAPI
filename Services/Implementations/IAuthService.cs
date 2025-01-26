


using Models.Entities;

namespace Services.Implementations
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string emailOrAlias, string password);
        string GenerateJwtToken(User user);
    }
}
