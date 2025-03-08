using DataAccess.DAOs.Interfaces;
using Models.DTOs.User;
using Models.Entities;
using Services.Helpers;
using Microsoft.Extensions.Configuration;
using Models.Enums;
using Config;
using static Models.Exceptions.CustomException;
using Services.Interfaces;
using System.Diagnostics.Metrics;


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

        public async Task<UserRequestDto> Register(PlayerRegisterRequestDto dto)
        {
            await ValidateAliasAsync(dto.Alias);
            await ValidateEmailAsync(dto.Email);

            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            var user = new UserRequestDto
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


        public async Task<UserRequestDto> CreateUserByAdmin(AdminRegisterRequestDto dto, int adminId)
        {
            // Validar que el admin que realiza la acción exista y tenga rol de Admin
            var admin = await _userDao.GetUserByIdAsync(adminId);
            if (admin == null || admin.Role != RoleEnum.Admin)
                throw new ForbiddenException(ErrorMessages.AccesDenied);

            if (dto.Role is RoleEnum.Admin or RoleEnum.Organizer or RoleEnum.Judge && admin.Role != RoleEnum.Admin)
                throw new ForbiddenException(ErrorMessages.AccesDenied);

            if (admin.Role == RoleEnum.Organizer && dto.Role != RoleEnum.Judge)
                throw new ForbiddenException(ErrorMessages.AccesDenied);

            await ValidateAliasAsync(dto.Alias);
            await ValidateEmailAsync(dto.Email);
            await ValidateCountryAsync(dto.CountryCode);

            // Validar existencia del país
            bool countryExists = await _countryDao.CountryExists(dto.CountryCode);
            if (!countryExists)
                throw new ValidationException(ErrorMessages.InvalidCountryCode);

            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            var user = new UserRequestDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CountryCode = dto.CountryCode,
                AvatarUrl = dto.AvatarUrl,
                Role = dto.Role,
                CreatedBy = adminId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userDao.AddUserAsync(user);
            return user;
        }

        private async Task ValidateAliasAsync(string alias)
        {
            var existingAlias = await _userDao.GetUserByAliasAsync(alias) != null ?
                throw new ValidationException(ErrorMessages.AliasAlreadyUse)
                : true;
        }

        private async Task ValidateEmailAsync(string email)
        {
            var existingEmailUser = await _userDao.GetActiveUserByEmailAsync(email) != null ?
                throw new ValidationException(ErrorMessages.EmailAlreadyUse)
                : true;
        }

        private async Task ValidateCountryAsync(string country)
        {
            bool countryExists = await _countryDao.CountryExists(country);
            if (!countryExists)
                throw new ValidationException(ErrorMessages.InvalidCountryCode);
        }
        
        public async Task<UserResponseDto> UpdateUser(int id, UserUpdateRequestDto dto)
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

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
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

        public async Task<UserResponseDto> GetUserById(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null || !user.IsActive)
                throw new NotFoundException(ErrorMessages.UserNotFound);

            return user;
        }

        public async Task DeletePermanentUser(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            await _userDao.PermanentDeleteUserAsync(id);
        }
    }
}
