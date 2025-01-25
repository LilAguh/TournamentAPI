using DataAccess.DAOs.Interfaces;
using Models.DTOs;
using Models.Entities;
using Services.Implementations;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly PasswordHasher _passwordHasher;

        public UserService(IUserDao userDao, PasswordHasher passwordHasher)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> Register(PlayerRegisterDto dto)
        {
            var existingUser = await _userDao.GetUserByEmailOrAliasAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email or alias already exists.");

            var userDto = new CreateUserDto
            {
                Role = "Player",  // Valor predeterminado
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                CountryCode = dto.CountryCode,
                AvatarUrl = dto.AvatarUrl,  // Cambiado de ImageUrl a AvatarUrl
                CreatedAt = DateTime.UtcNow,  // Valor predeterminado
                IsActive = true,  // Valor predeterminado
                CreatedBy = 0  // Valor predeterminado
            };

            await _userDao.AddUserAsync(userDto);
            return await _userDao.GetUserByEmailOrAliasAsync(dto.Email);
        }
    }
}
