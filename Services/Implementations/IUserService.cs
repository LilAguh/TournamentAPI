using Models.DTOs;
using Models.Entities;
using Services.Interfaces;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public interface IUserService
    {
        Task<User> Register(PlayerRegisterDto dto);
        Task<User> CreateUserByAdmin(AdminRegisterDto dto, int adminId);
        Task<User> UpdateUser(int id, UpdateUserDto dto);
        Task DeleteUser(int id);
        Task<User> GetUserById(int id);
    }
}
