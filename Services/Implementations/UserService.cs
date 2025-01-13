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
using Models.Exceptions;
using System.Net.WebSockets;

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

        public async Task<string> Login(UserDto userDto)
        {
            var user = await _userDao.GetUserByEmail(userDto.Email);
            
            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            if (!_passwordHasher.VerifyPassword(userDto.Password, user.Password))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            var token = _jwtHelper.GenerateToken(userDto);
            return token;
        }

        public async Task<UserResponseDto> GetUserById(int id)
        {
            var user = await _userDao.GetUserById(id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

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

        public async Task UpdateUser(int id, UserDto userDto)
        {
            var user = await _userDao.GetUserById(id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var passwordHash = _passwordHasher.HashPassword(userDto.Password);

            var updatedUser = new UserDto
            {
                Id = id, // Asegúrate de incluir el Id
                Name = userDto.Name ?? user.Name,
                LastName = userDto.LastName ?? user.LastName,
                Alias = userDto.Alias ?? user.Alias,
                Email = userDto.Email ?? user.Email,
                Password = passwordHash,
                Country = userDto.Country ?? user.Country,
                Role = userDto.Role ?? user.Role,
                AvatarUrl = userDto.AvatarUrl ?? user.AvatarUrl
            };

            await _userDao.UpdateUser(id, updatedUser);
        }

        public async Task DesactivateUser(int id)
        {
            var user = await _userDao.GetUserById(id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            await _userDao.DesactivateUser(id);
        }
    }
}
