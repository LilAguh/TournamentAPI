
using Core.DTOs;
using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task<User> GetUserByEmailOrAliasAsync(string emailOrAlias);
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task AddUserAsync(PlayerRegisterDto dto);
        Task UpdateLastLoginAsync(int userId);
        Task UpdateUserAsync(User user);
        Task UpdateUserStatusAsync(User user);
    }
}
