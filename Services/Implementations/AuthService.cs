

using DataAccess.DAOs.Implementations;
using DataAccess.DAOs.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Models.Entities;
using Services.Helpers;
using Models.Exceptions;
using Config;
using static Models.Exceptions.CustomException;
using Models.DTOs.User;

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

        public async Task<UserResponseDto> AuthenticateAsync(string identifier, string password)
        {
            var user = await _userDao.GetUserByAliasAsync(identifier);

            if (user != null)
            {
                if (!user.IsActive)
                    throw new UnauthorizedException("Usuario no activo");
                _passwordHasher.VerifyPassword(password, user.PasswordHash);
                return user;
            }
            else
            {
                user = await _userDao.GetActiveUserByEmailAsync(identifier);
                if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
                    throw new UnauthorizedException("Credenciales inválidas");
                return user;
            }
        }

        public string GenerateJwtToken(UserResponseDto user)
        {
            return _jwtHelper.GenerateToken(user);
        }
    }
}
