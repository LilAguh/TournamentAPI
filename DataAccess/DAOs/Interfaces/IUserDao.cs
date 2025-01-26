
using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task AddUserAsync(User user);
        Task<User> GetUserByEmailOrAliasAsync(string emailOrAlias);
    }
}
