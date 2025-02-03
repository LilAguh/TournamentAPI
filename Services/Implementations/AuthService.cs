

using DataAccess.DAOs.Implementations;
using DataAccess.DAOs.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Models.Entities;
using Services.Helpers;
using Models.Exceptions;
using Config;
using static Models.Exceptions.CustomException;

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

        public async Task<User> AuthenticateAsync(string identifier, string password)
        {
            // Primero se intenta buscar el usuario por alias
            var user = await _userDao.GetUserByAliasAsync(identifier);

            // Si se encontró por alias, se verifica si está activo
            if (user != null)
            {
                if (!user.IsActive)
                    throw new UnauthorizedException("Usuario no activo");
                if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
                    throw new UnauthorizedException("Credenciales inválidas");
                return user;
            }
            else
            {
                // Si no se encontró por alias, se busca por email (solo entre activos)
                user = await _userDao.GetActiveUserByEmailAsync(identifier);
                if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
                    throw new UnauthorizedException("Credenciales inválidas");
                return user;
            }
        }

        public string GenerateJwtToken(User user)
        {
            return _jwtHelper.GenerateToken(user);
        }
    }
}
