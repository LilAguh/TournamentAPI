using DataAccess.DAOs.Interfaces;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Helpers;
using Models.DTOs;
using System.Linq.Expressions;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly PasswordHasher _passwordHasher;
        private readonly JwtHelper _jwtHelper;

        public UserService(IUserDao userDao, PasswordHasher passwordHasher, JwtHelper jwtHelper)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _jwtHelper = jwtHelper;
        }

        public async Task<UserResponseDto> Register(UserDto userDto)
        {
            var existingUser = await _userDao.GetUserByEmail(userDto.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already registered");
            }

            var passwordHash = _passwordHasher.HashPassword(userDto.Password);

            var newUser = new UserDto
            {
                Name = userDto.Name,
                LastName = userDto.LastName,
                Alias = userDto.Alias,
                Email = userDto.Email,
                Password = passwordHash,
                Country = userDto.Country,
                Role = userDto.Role,
                AvatarUrl = userDto.AvatarUrl,
                Active = true
            };

            await _userDao.AddUser(newUser);

            var user = await _userDao.GetUserByEmail(userDto.Email);
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Alias = user.Alias,
                Email = user.Email,
                Country = user.Country,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl
            };
        }
    }
}
