
using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task<User> GetUserByEmailOrAliasAsync(string emailOrAlias);
        Task<User> GetUserByIdAsync(int id); // Nuevo método
        Task AddUserAsync(User user);
        Task UpdateLastLoginAsync(int userId);
    }
}
