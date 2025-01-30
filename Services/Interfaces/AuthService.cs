

using DataAccess.DAOs.Implementations;
using DataAccess.DAOs.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Models.Entities;
using Services.Helpers;
using Models.Exceptions;
using Config;

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
            var user = await _userDao.GetUserByEmailOrAliasAsync(emailOrAlias);
            if (user == null)
            {
                throw new ArgumentException(ErrorMessages.InvalidCredentials);
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
                throw new ArgumentException(ErrorMessages.InvalidCredentials);

            return user;
        }

        public string GenerateJwtToken(User user)
        {
            return _jwtHelper.GenerateToken(user);
        }
    }
}
