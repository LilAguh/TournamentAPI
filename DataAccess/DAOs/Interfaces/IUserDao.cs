
using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task AddUserAsync(CreateUserDto userDto);
        Task<User> GetUserByEmailOrAliasAsync(string emailOrAlias);
    }
}
