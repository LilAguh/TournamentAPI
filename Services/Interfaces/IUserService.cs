using Models.DTOs;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> Register(UserDto userDto);
        Task<string> Login(UserLoginDto userLoginDto);
        Task<UserResponseDto> GetUserById(int id);
        Task UpdateUser(int id, UserDto userDto);
        Task DeactivateUser(int id);
    }
}
