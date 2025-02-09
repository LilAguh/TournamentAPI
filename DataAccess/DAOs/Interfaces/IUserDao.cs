
using Models.DTOs;
using Models.DTOs.User;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task<UserResponseDto> GetUserByAliasAsync(string alias);
        Task<UserResponseDto> GetActiveUserByEmailAsync(string email);
        Task<UserResponseDto> GetUserByIdentifierAsync(string identifier);
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateLastLoginAsync(int userId);
        Task UpdateUserAsync(User user);
        Task UpdateUserStatusAsync(User user);
    }
}
