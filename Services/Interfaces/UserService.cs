using DataAccess.DAOs.Interfaces;
using Models.DTOs;
using Models.Entities;
using Services.Implementations;
using Services.Helpers;
using Microsoft.Extensions.Configuration;


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
            var existingUser = await _userDao.GetUserByEmailOrAliasAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email or alias already exists.");

            var countryExists = await _countryDao.CountryExists(dto.CountryCode);
            if (!countryExists)
                throw new ArgumentException("País no válido");

            var hashedPassword = _passwordHasher.HashPassword(dto.Password);


            var defaultAvatar = _config["DefaultSettings:AvatarUrl"];
            var avatarUrl = !string.IsNullOrEmpty(dto.AvatarUrl)? dto.AvatarUrl : defaultAvatar;
            if (!Uri.IsWellFormedUriString(avatarUrl, UriKind.Absolute))
                throw new ArgumentException("URL de avatar inválida");

            var user = new User
            {
                Role = "Player",  // Valor predeterminado
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CountryCode = dto.CountryCode,
                AvatarUrl = avatarUrl,  // Cambiado de ImageUrl a AvatarUrl
                CreatedAt = DateTime.UtcNow,  // Valor predeterminado
                IsActive = true,  // Valor predeterminado
                CreatedBy = 0  // Valor predeterminado
            };

            await _userDao.AddUserAsync(user);

            return user;
        }
    }
}
