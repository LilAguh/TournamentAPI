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
            // Validar que el email o alias no estén en uso
            var existingUser = await _userDao.GetUserByEmailOrAliasAsync(dto.Email);
            if (existingUser != null)
                throw new ArgumentException("El email o alias ya está en uso.");

            // Generar el hash de la contraseña
            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            // Crear el usuario
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CountryCode = dto.CountryCode,
                AvatarUrl = dto.AvatarUrl,
                Role = RoleEnum.Player, // Rol predeterminado (Player = 1)
                CreatedAt = DateTime.UtcNow,
                IsActive = true, // esto en un futuro se tiene que cambiar por un false, que se active con un mail
                CreatedBy = 0 // Creado por el sistema
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

            // Validar que el rol es válido
            if (!Enum.IsDefined(typeof(RoleEnum), dto.Role))
            {
                throw new ArgumentException("Rol inválido. Los roles válidos son 1 (Player), 2 (Admin), 3 (Judge), 4 (Organizer).");
            }

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
                Role = dto.Role, // Rol asignado por el admin
                CreatedBy = adminId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userDao.AddUserAsync(user);
            return user;
        }
    }
}
