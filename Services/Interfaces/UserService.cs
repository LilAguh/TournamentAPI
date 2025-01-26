using DataAccess.DAOs.Interfaces;
using Models.DTOs;
using Models.Entities;
using Services.Implementations;
using Services.Helpers;
using Microsoft.Extensions.Configuration;
using Models.Enums;


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
            // Generar el hash de la contraseña
            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            // Crear el usuario
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword, // Asegúrate de asignar el hash
                CountryCode = dto.CountryCode,
                Role = RoleEnum.Player, // Rol predeterminado
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Guardar en la base de datos
            await _userDao.AddUserAsync(user);

            return user;
        }

        public async Task<User> CreateUserByAdmin(AdminRegisterDto dto, int adminId)
        {
            // Validar que el admin existe y tiene el rol correcto
            var admin = await _userDao.GetUserByIdAsync(adminId);
            if (admin == null || admin.Role != RoleEnum.Admin)
                throw new UnauthorizedAccessException("Acceso denegado");

            // Validar alias/email único
            var existingUser = await _userDao.GetUserByEmailOrAliasAsync(dto.Email);
            if (existingUser != null)
                throw new ArgumentException("El email o alias ya existe");

            // Hash de la contraseña
            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            // Crear usuario
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
    }
}
