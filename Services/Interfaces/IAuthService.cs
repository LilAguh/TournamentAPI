


using Models.DTOs;
using Models.Entities;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDto> AuthenticateAsync(string emailOrAlias, string password);
        string GenerateJwtToken(UserResponseDto user);
    }
}
