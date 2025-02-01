using Models.DTOs;
using Models.Entities;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Register(PlayerRegisterDto dto);
        Task<User> CreateUserByAdmin(AdminRegisterDto dto, int adminId);
        Task<User> UpdateUser(int id, UpdateUserDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task DeleteUser(int id);
        Task<User> GetUserById(int id);
    }
}
