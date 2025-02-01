
using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task<User> GetUserByEmailOrAliasAsync(string emailOrAlias);
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateLastLoginAsync(int userId);
        Task UpdateUserAsync(User user);
        Task UpdateUserStatusAsync(User user);
    }
}
