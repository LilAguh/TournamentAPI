﻿using Models.DTOs;
using Models.Entities;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public interface IUserService
    {
        Task<User> Register(PlayerRegisterDto dto);
        Task<User> CreateUserByAdmin(AdminRegisterDto dto, int adminId); // Nuevo método
    }
}
