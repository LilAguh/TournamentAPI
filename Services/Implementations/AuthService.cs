﻿


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
            var user = await _userDao.GetUserByEmailOrAliasAsync(emailOrAlias);
            if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
                throw new UnauthorizedException(ErrorMessages.InvalidCredentials);

            return user;
        }

        public string GenerateJwtToken(UserResponseDto user)
        {
            return _jwtHelper.GenerateToken(user);
        }
    }
}
