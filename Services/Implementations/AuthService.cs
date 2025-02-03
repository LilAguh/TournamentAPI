


using DataAccess.DAOs.Interfaces;
using Services.Interfaces;
using Models.Entities;
using Services.Helpers;
using Config;
using static Models.Exceptions.CustomException;
using Models.DTOs;

namespace Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserDao _userDao;
        private readonly PasswordHasher _passwordHasher;
        private readonly JwtHelper _jwtHelper;

        public AuthService(
            IUserDao userDao,
            PasswordHasher passwordHasher,
            JwtHelper jwtHelper)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _jwtHelper = jwtHelper;
        }

        public async Task<UserResponseDto> AuthenticateAsync(string emailOrAlias, string password)
        {
            if (string.IsNullOrEmpty(emailOrAlias))
                throw new ArgumentNullException(nameof(emailOrAlias), "El correo electrónico o alias no puede ser nulo o vacío.");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "La contraseña no puede ser nula o vacía.");

            var user = await _userDao.GetUserByEmailOrAliasAsync(emailOrAlias);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                // Corregir automáticamente el PasswordHash nulo
                user.PasswordHash = _passwordHasher.HashPassword(password);
                await _userDao.UpdateUserAsync(new UserUpdateDto
                {
                    Id = user.Id,
                    PasswordHash = user.PasswordHash
                });
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
                throw new UnauthorizedException("Contraseña incorrecta.");

            return user;
        }

        public string GenerateJwtToken(UserResponseDto user)
        {
            return _jwtHelper.GenerateToken(user);
        }
    }
}
