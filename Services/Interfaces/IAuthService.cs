


using Models.DTOs.User;
using Models.Entities;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> AuthenticateAsync(string identifier, string password);
        string GenerateJwtToken(UserResponseDto user);
    }
}
