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
            // Validar si el email o alias ya están en uso
            if (await _userDao.GetUserByEmailOrAliasAsync(dto.Email) != null)
                throw new ArgumentException("El email o alias ya está en uso.");

            // Crear el hash de la contraseña
            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            // Crear el objeto del usuario
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CountryCode = dto.CountryCode,
                AvatarUrl = dto.AvatarUrl,
                Role = RoleEnum.Player, // Rol predeterminado para el registro
                CreatedAt = DateTime.UtcNow,
                IsActive = true // Cambiar a false si se quiere activar por correo más adelante
            };

            // Guardar en la base de datos
            await _userDao.AddUserAsync(user);

            return user;
        }

        public async Task<User> CreateUserByAdmin(AdminRegisterDto dto, int adminId)
        {
            // Validar que el admin existe y tiene rol correcto
            var admin = await _userDao.GetUserByIdAsync(adminId);
            if (admin == null || admin.Role != RoleEnum.Admin)
                throw new UnauthorizedAccessException("Acceso denegado.");

            // Validar alias/email único
            if (await _userDao.GetUserByEmailOrAliasAsync(dto.Email) != null)
                throw new ArgumentException("El email o alias ya existe.");

            // Crear el hash de la contraseña
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
                Role = dto.Role, // Asignar rol especificado por el admin
                CreatedBy = adminId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userDao.AddUserAsync(user);

            return user;
        }

        public async Task<User> UpdateUser(int id, UpdateUserDto dto)
        {
            // Obtener el usuario por ID
            var user = await _userDao.GetUserByIdAsync(id);
            if (user == null)
                throw new ArgumentException("El usuario no existe.");

            // Actualizar los campos permitidos
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.CountryCode = dto.CountryCode ?? user.CountryCode;
            user.AvatarUrl = dto.AvatarUrl ?? user.AvatarUrl;

            // Guardar cambios en la base de datos
            await _userDao.UpdateUserAsync(user);

            return user;
        }
    }
}
