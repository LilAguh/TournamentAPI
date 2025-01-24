using Models.DTOs;
using Shared;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> Register(UserDto userDto);
        Task<ApiResponse<UserResponseDto>> RegisterPlayer(PlayerRegisterDto playerRegisterDto);
        Task<UserResponseDto> RegisterAdmin(AdminsRegisterDto adminsRegisterDto, int createdBy);
        Task<string> Login(UserLoginDto userLoginDto);
        Task<UserResponseDto> GetUserById(int id);
        Task UpdateUser(int id, UserDto userDto);
        Task DeactivateUser(int id);
    }
}
