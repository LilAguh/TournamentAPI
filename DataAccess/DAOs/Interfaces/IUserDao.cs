﻿
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
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task AddUserAsync(UserRequestDto userDto);
        Task UpdateLastLoginAsync(int userId);
        Task UpdateUserAsync(UserResponseDto userDto);
        Task UpdateUserStatusAsync(UserResponseDto userDto);
        Task<bool> PermanentDeleteUserAsync(int userId);
    }
}
