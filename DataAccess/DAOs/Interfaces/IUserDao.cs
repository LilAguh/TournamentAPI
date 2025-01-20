using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task AddUser(UserDto userRegisterDto);
        Task UpdateUser(int id, UserDto userUpdateDto);
        Task DeactivateUser(int id);
    }
}
