

using DataAccess.DAOs.Implementations;
using DataAccess.DAOs.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Models.Entities;
using DataAccess.DAOs.Interfaces;
using Services.Helpers;
using Models.Exceptions;

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

        public async Task<User> AuthenticateAsync(string emailOrAlias, string password)
        {
            // Obtener el usuario
            var user = await _userDao.GetUserByEmailOrAliasAsync(emailOrAlias);
            if (user == null)
            {
                throw new ArgumentException("Usuario no encontrado.");
            }

            // Verificar que el hash no sea nulo
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new ArgumentException("El hash de la contraseña no está configurado.");
            }

            // Verificar la contraseña
            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                throw new ArgumentException("Credenciales inválidas.");
            }

            return user;
        }

        public string GenerateJwtToken(User user)
        {
            return _jwtHelper.GenerateToken(user);
        }
    }
}
