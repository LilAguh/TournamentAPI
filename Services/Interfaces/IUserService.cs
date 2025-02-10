using Models.DTOs;
using Models.DTOs.User;
using Models.Entities;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<UserRequestDto> Register(PlayerRegisterDto dto);
        Task<UserRequestDto> CreateUserByAdmin(AdminRegisterDto dto, int adminId);
        Task<UserResponseDto> UpdateUser(int id, UpdateUserDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task DeleteUser(int id);
        Task<UserResponseDto> GetUserById(int id);
    }
}
