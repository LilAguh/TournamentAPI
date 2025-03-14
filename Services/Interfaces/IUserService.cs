﻿using Models.DTOs.User;
using Models.Entities;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<UserRequestDto> Register(UserRegisterRequestDto dto);
        Task<UserRequestDto> CreateUserByAdmin(UserRegisterRequestDto dto, int adminId);
        Task<UserResponseDto> UpdateUser(int id, UserUpdateRequestDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto);
        Task DeleteUser(int id);
        Task<UserResponseDto> GetUserById(int id);
        Task DeletePermanentUser(int id);
    }
}
