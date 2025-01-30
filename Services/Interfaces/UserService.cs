using DataAccess.DAOs.Interfaces;
using Models.DTOs;
using Models.Entities;
using Services.Implementations;
using Services.Helpers;
using Microsoft.Extensions.Configuration;
using Models.Enums;
using Config;


namespace Services.Interfaces
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly PasswordHasher _passwordHasher;
        private readonly ICountryDao _countryDao;
        private readonly IConfiguration _config;

        public UserService(IUserDao userDao, PasswordHasher passwordHasher, ICountryDao countryDao, IConfiguration config)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _countryDao = countryDao;
            _config = config;
        }

        public async Task<User> Register(PlayerRegisterDto dto)
        {
            if (await _userDao.GetUserByEmailOrAliasAsync(dto.Email) != null)
                throw new ArgumentException(ErrorMessages.DataUserAlreadyUse);

            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CountryCode = dto.CountryCode,
                AvatarUrl = dto.AvatarUrl,
                Role = RoleEnum.Player,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userDao.AddUserAsync(user);

            return user;
        }

        public async Task<User> CreateUserByAdmin(AdminRegisterDto dto, int adminId)
        {
            var admin = await _userDao.GetUserByIdAsync(adminId);
            if (admin == null || admin.Role != RoleEnum.Admin)
                throw new UnauthorizedAccessException(ErrorMessages.AccesDenied);

            if (await _userDao.GetUserByEmailOrAliasAsync(dto.Email) != null)
                throw new ArgumentException(ErrorMessages.DataUserAlreadyUse);

            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CountryCode = dto.CountryCode,
                Role = dto.Role,
                CreatedBy = adminId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userDao.AddUserAsync(user);

            return user;
        }

        public async Task<User> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null)
                throw new ArgumentException(ErrorMessages.UserNotFound);

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.CountryCode = dto.CountryCode ?? user.CountryCode;
            user.AvatarUrl = dto.AvatarUrl ?? user.AvatarUrl;

            await _userDao.UpdateUserAsync(user);

            return user;
        }

        public async Task DeleteUser(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null)
                throw new ArgumentException(ErrorMessages.UserNotFound);

            user.IsActive = false;
            await _userDao.UpdateUserStatusAsync(user);
        }
    }
}
