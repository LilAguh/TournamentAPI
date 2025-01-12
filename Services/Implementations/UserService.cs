using DataAccess.DAOs.Interfaces;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Helpers;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly PasswordHasher _passwordHasher;
        private readonly JwtHelper _jwtHelper;

        public UserService(IUserDao userDao, PasswordHasher passwordHasher, JwtHelper jwtHelper)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _jwtHelper = jwtHelper;
        }
    }
}
