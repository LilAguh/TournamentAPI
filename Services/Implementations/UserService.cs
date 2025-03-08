using DataAccess.DAOs.Interfaces;
using Models.DTOs.User;
using Services.Helpers;
using Models.Enums;
using Config;
using Services.Interfaces;
using static Models.Exceptions.CustomException;


namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly PasswordHasher _passwordHasher;
        private readonly ICountryService _countryService;

        public UserService(IUserDao userDao, PasswordHasher passwordHasher, ICountryService countryService)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _countryService = countryService;
        }

        public async Task<UserRequestDto> Register(PlayerRegisterRequestDto dto)
        {
            await ValidateAliasAsync(dto.Alias);
            await ValidateEmailAsync(dto.Email);
            await _countryService.ValidateCountryAsync(dto.CountryCode);
            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            var user = new UserRequestDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CountryCode = dto.CountryCode,
                AvatarUrl = dto.AvatarUrl,
                Role = RoleEnum.Player,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userDao.AddUserAsync(user);
            return user;
        }


        public async Task<UserRequestDto> CreateUserByAdmin(AdminRegisterRequestDto dto, int adminId)
        {
            // Validar que el admin que realiza la acción exista y tenga rol de Admin
            var admin = await _userDao.GetUserByIdAsync(adminId);
            if (admin == null || admin.Role != RoleEnum.Admin)
                throw new ForbiddenException(ErrorMessages.AccesDenied);

            if (dto.Role is RoleEnum.Admin or RoleEnum.Organizer or RoleEnum.Judge && admin.Role != RoleEnum.Admin)
                throw new ForbiddenException(ErrorMessages.AccesDenied);

            if (admin.Role == RoleEnum.Organizer && dto.Role != RoleEnum.Judge)
                throw new ForbiddenException(ErrorMessages.AccesDenied);

            await ValidateAliasAsync(dto.Alias);
            await ValidateEmailAsync(dto.Email);

            await _countryService.ValidateCountryAsync(dto.CountryCode);
            var hashedPassword = _passwordHasher.HashPassword(dto.Password);

            var user = new UserRequestDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CountryCode = dto.CountryCode,
                AvatarUrl = dto.AvatarUrl,
                Role = dto.Role,
                CreatedBy = adminId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userDao.AddUserAsync(user);
            return user;
        }

        public async Task<UserResponseDto> UpdateUser(int id, UserUpdateRequestDto dto)
        {
            var user = await ValidateUserExistsAsync(id);

            if (!string.IsNullOrEmpty(dto.CountryCode))
            {
                await _countryService.ValidateCountryAsync(dto.CountryCode);
            }

            user.FirstName = !string.IsNullOrEmpty(dto.FirstName) ? dto.FirstName : user.FirstName;
            user.LastName = !string.IsNullOrEmpty(dto.LastName) ? dto.LastName : user.LastName;
            user.Alias = !string.IsNullOrEmpty(dto.Alias) ? dto.Alias : user.Alias;
            user.Email = !string.IsNullOrEmpty(dto.Email) ? dto.Email : user.Email;
            user.CountryCode = !string.IsNullOrEmpty(dto.CountryCode) ? dto.CountryCode : user.CountryCode;
            user.AvatarUrl = !string.IsNullOrEmpty(dto.AvatarUrl) ? dto.AvatarUrl : user.AvatarUrl;

            await _userDao.UpdateUserAsync(user);
            return user;
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
        {
            var user = await ValidateUserExistsAsync(userId);

            if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new ValidationException(ErrorMessages.InvalidCredentials);

            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            await _userDao.UpdateUserAsync(user);
        }

        public async Task DeleteUser(int id)
        {
            var user = await ValidateUserExistsAsync(id);

            user.IsActive = false;
            await _userDao.UpdateUserStatusAsync(user);
        }

        public async Task<UserResponseDto> GetUserById(int id)
        {
            var user = await ValidateUserExistsAsync(id);

            return user;
        }

        // Private section //

        public async Task DeletePermanentUser(int id)
        {
            var user = await ValidateUserExistsAsync(id);
            await _userDao.PermanentDeleteUserAsync(id);
        }

        private async Task ValidateAliasAsync(string alias)
        {
            var existingAlias = await _userDao.GetUserByAliasAsync(alias) != null ?
                throw new ValidationException(ErrorMessages.AliasAlreadyUse)
                : true;
        }

        private async Task ValidateEmailAsync(string email)
        {
            var existingEmailUser = await _userDao.GetActiveUserByEmailAsync(email) != null ?
                throw new ValidationException(ErrorMessages.EmailAlreadyUse)
                : true;
        }

        private async Task<UserResponseDto> ValidateUserExistsAsync(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            return user ?? throw new NotFoundException(ErrorMessages.UserNotFound);
        }
    }
}
