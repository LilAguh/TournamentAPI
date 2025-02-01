using DataAccess.DAOs.Interfaces;
using Models.DTOs;
using Models.Entities;
using Services.Helpers;
using Microsoft.Extensions.Configuration;
using Models.Enums;
using Config;
using static Models.Exceptions.CustomException;
using Services.Interfaces;


namespace Services.Implementations
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
                throw new ValidationException(ErrorMessages.DataUserAlreadyUse);

            bool countryExists = await _countryDao.CountryExists(dto.CountryCode);
            if (!countryExists)
                throw new ValidationException(ErrorMessages.InvalidCountryCode);

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
                throw new ForbiddenException(ErrorMessages.AccesDenied);

            if (dto.Role is RoleEnum.Admin or RoleEnum.Organizer or RoleEnum.Judge
                && admin.Role != RoleEnum.Admin)
            {
                throw new ForbiddenException(ErrorMessages.AccesDenied);
            }

            if (admin.Role == RoleEnum.Organizer && dto.Role != RoleEnum.Judge)
                throw new ForbiddenException(ErrorMessages.AccesDenied);

            bool countryExists = await _countryDao.CountryExists(dto.CountryCode);
            if (!countryExists)
                throw new ValidationException(ErrorMessages.InvalidCountryCode);

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
                throw new NotFoundException(ErrorMessages.UserNotFound);

            if (!string.IsNullOrEmpty(dto.CountryCode))
            {
                bool countryExists = await _countryDao.CountryExists(dto.CountryCode);
                if (!countryExists)
                    throw new ValidationException(ErrorMessages.InvalidCountryCode);
            }

            user.FirstName = !string.IsNullOrEmpty(dto.FirstName) ? dto.FirstName : user.FirstName;
            user.LastName = !string.IsNullOrEmpty(dto.LastName) ? dto.LastName : user.LastName;
            user.Alias = !string.IsNullOrEmpty(dto.Alias) ? dto.Alias : user.Alias;
            user.Email = !string.IsNullOrEmpty(dto.Email) ? dto.Email : user.Email;
            user.CountryCode = !string.IsNullOrEmpty(dto.CountryCode) ? dto.CountryCode : user.CountryCode;
            user.AvatarUrl = !string.IsNullOrEmpty(dto.AvatarUrl) ? dto.AvatarUrl : user.AvatarUrl;

            await _userDao.UpdateUserAsync(user);
            return user;
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userDao.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException(ErrorMessages.UserNotFound);

            if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new ValidationException(ErrorMessages.InvalidCredentials);

            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            await _userDao.UpdateUserAsync(user);
        }

        public async Task DeleteUser(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null)
                throw new NotFoundException(ErrorMessages.UserNotFound);

            user.IsActive = false;
            await _userDao.UpdateUserStatusAsync(user);
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null || !user.IsActive)
                throw new NotFoundException(ErrorMessages.UserNotFound);

            return user;
        }
    }
}
