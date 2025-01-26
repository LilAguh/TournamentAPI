

using DataAccess.DAOs.Implementations;
using DataAccess.DAOs.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using Models.Entities;
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
            var user = await _userDao.GetUserByEmailOrAliasAsync(emailOrAlias);

            if (user == null)
            {
                Console.WriteLine("Usuario no encontrado.");
                throw new ArgumentException("Credenciales inválidas.");
            }

            Console.WriteLine(user.Alias, user.FirstName, user.PasswordHash);

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                Console.WriteLine("PasswordHash es nulo o vacío.");
                throw new ArgumentException("El hash de la contraseña no está configurado.");
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                Console.WriteLine("La contraseña no coincide.");
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
