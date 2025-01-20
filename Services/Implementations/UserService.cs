using DataAccess.DAOs.Interfaces;
using Services.Interfaces;
using Services.Helpers;
using Models.DTOs;
using Models.Exceptions;

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

                var passwordHash = _passwordHasher.HashPassword(userDto.Password);

                var newUser = new UserDto
                {
                    Name = userDto.Name,
                    LastName = userDto.LastName,
                    Alias = userDto.Alias,
                    Email = userDto.Email,
                    Password = passwordHash,
                    Country = userDto.Country,
                    Role = userDto.Role,
                    Avatar = userDto.Avatar,
                    Active = true,
                    CreatedBy = userDto.CreatedBy // Use the value provided in userDto
                };

                await _userDao.AddUser(newUser);

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
                    Avatar = user.Avatar
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
