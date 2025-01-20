using DataAccess.DAOs.Interfaces;
using Services.Interfaces;
using Services.Helpers;
using Models.DTOs;
using Models.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

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

        public async Task<UserResponseDto> Register(UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto), "UserDto object cannot be null.");

            try
            {
                var existingUser = await _userDao.GetUserByEmail(userDto.Email);
                if (existingUser != null)
                {
                    throw new Exception("The email is already registered.");
                }

                userDto.Password = _passwordHasher.HashPassword(userDto.Password); // Hashear contraseña
                userDto.Role = "player"; // Asignar rol predeterminado
                userDto.Active = true;


                await _userDao.AddUser(userDto);
                Console.Write(userDto);

                var user = await _userDao.GetUserByEmail(userDto.Email);
                return new UserResponseDto
                {
                    ID = user.ID,
                    Name = user.Name,
                    LastName = user.LastName,
                    Alias = user.Alias,
                    Email = user.Email,
                    Country = user.Country,
                    Role = user.Role,
                    Avatar = user.Avatar,
                    CreatedBy = user.CreatedBy
                };
            }
            catch (Exception ex)
            {
                // Catch specific exceptions (e.g., duplicate alias)
                if (ex.Message.Contains("Duplicate entry") && ex.Message.Contains("Alias"))
                {
                    throw new Exception("The alias is already in use.");
                }
                throw; // Rethrow other exceptions
            }
        }

        public async Task<string> Login(UserLoginDto userLoginDto)
        {
            var user = await _userDao.GetUserByEmail(userLoginDto.Email);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            if (!_passwordHasher.VerifyPassword(userLoginDto.Password, user.Password))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            var userDto = new UserDto
            {
                Email = user.Email,
                Role = user.Role
            };

            var token = _jwtHelper.GenerateToken(userDto);
            return token;
        }

        public async Task<UserResponseDto> GetUserById(int id)
        {
            var user = await _userDao.GetUserById(id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return new UserResponseDto
            {
                ID = user.ID,
                Name = user.Name,
                LastName = user.LastName,
                Alias = user.Alias,
                Email = user.Email,
                Country = user.Country,
                Role = user.Role,
                Avatar = user.Avatar
            };
        }

        public async Task UpdateUser(int id, UserDto userDto)
        {
            var user = await _userDao.GetUserById(id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var passwordHash = _passwordHasher.HashPassword(userDto.Password);

            var updatedUser = new UserDto
            {
                ID = id, // Asegúrate de incluir el Id
                Name = userDto.Name ?? user.Name,
                LastName = userDto.LastName ?? user.LastName,
                Alias = userDto.Alias ?? user.Alias,
                Email = userDto.Email ?? user.Email,
                Password = passwordHash,
                Country = userDto.Country ?? user.Country,
                Role = userDto.Role ?? user.Role,
                Avatar = userDto.Avatar ?? user.Avatar
            };

            await _userDao.UpdateUser(id, updatedUser);
        }

        public async Task DeactivateUser(int id)
        {
            var user = await _userDao.GetUserById(id);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            await _userDao.DeactivateUser(id);
        }
    }
}
