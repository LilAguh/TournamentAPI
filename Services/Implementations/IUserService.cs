using Models.DTOs;
using Models.Entities;

namespace Services.Implementations
{
    public interface IUserService
    {
        Task<User> Register(PlayerRegisterDto dto);
    }
}
