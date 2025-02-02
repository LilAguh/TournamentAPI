
using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task<UserResponseDto> GetUserByEmailOrAliasAsync(string emailOrAlias);
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task AddUserAsync(UserCreateDto user);
        Task UpdateLastLoginAsync(int userId);
        Task UpdateUserAsync(UserUpdateDto user);
        Task UpdateUserStatusAsync(int userId, bool isActive);
    }
}
