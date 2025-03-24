using DataAccess.DAOs.Interfaces;
using Models.DTOs.User;
using Services.Helpers;
using Models.Enums;
using Config;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    /// <summary>
    /// Servicio para gestionar las operaciones relacionadas con usuarios.
    /// Incluye funcionalidades como registro, actualización, cambio de contraseña,
    /// eliminación y consulta de usuarios.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly PasswordHasher _passwordHasher;
        private readonly ICountryService _countryService;

        public UserService(
            IUserDao userDao, 
            PasswordHasher passwordHasher, 
            ICountryService countryService)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _countryService = countryService;
        }

        /*
            "firstName": "Aguh",
            "lastName": "Ochoa",
            "alias": "AguhCab123123s",
            "email": "aguhOchoa123123s@gmail.com",
            "password": "Argentina14123123",
            "countryCode": "AR",
            "avatarUrl": "https://tn.com.ar/resizer/v2/guillermo-farre-sentencio-el-descenso-de-river-ap-C3BQEM6HE76CAM7ARY7ETWPG7Q.jpg?auth=84a0351a3871bb7f63c7762fe232d157d2349d146d2ad637b39d15b2a11cee6c&width=1023",
            "createdBy": 0
        */


        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="dto">DTO con los datos del usuario a registrar.</param>
        /// <param name="creatorId">ID del usuario que realiza el registro (opcional).</param>
        /// <returns>DTO con los datos del usuario registrado.</returns>
        /// <exception cref="ValidationException">Se lanza si el alias, email o código de país no son válidos.</exception>
        /// <exception cref="ForbiddenException">Se lanza si el usuario no tiene permisos para registrar otros usuarios.</exception>
        public async Task<UserResponseDto> Register(UserRegisterRequestDto dto, int? creatorId)
        {
            await ValidateUserDetailsAsync(dto.Alias, dto.Email, dto.CountryCode);
            var (role, createdBy) = await DetermineRoleAndCreator(dto, creatorId);

            var user = CreateUser(dto, role, createdBy);

            var userId = await _userDao.AddUserAsync(user);

            return MapUserResponseDto(user, userId);

        }


        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <param name="id">ID del usuario a actualizar.</param>
        /// <param name="dto">DTO con los nuevos datos del usuario.</param>
        /// <returns>DTO con los datos actualizados del usuario.</returns>
        /// <exception cref="NotFoundException">Se lanza si el usuario no existe.</exception>
        /// <exception cref="ValidationException">Se lanza si el código de país no es válido.</exception>
        public async Task<UserResponseDto> UpdateUser(int id, UserUpdateRequestDto dto)
        {
            var user = await ValidateUserExistsAsync(id);

            if (!string.IsNullOrEmpty(dto.CountryCode))
                await _countryService.ValidateCountryAsync(dto.CountryCode);

            UpdateUserProperties(user, dto);
            await _userDao.UpdateUserAsync(user);
            return user;
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
        {
            var user = await ValidateUserExistsAsync(userId);

            _passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash);
               
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

        public async Task DeletePermanentUser(int id)
        {
            var user = await ValidateUserExistsAsync(id);
            await _userDao.PermanentDeleteUserAsync(id);
        }

        // Private section //

        private async Task<(RoleEnum role, int createdBy)> DetermineRoleAndCreator(UserRegisterRequestDto dto, int? creatorId)
        {
            if (creatorId != null)
            {
                return PublicRegistration(dto);
            }

            var creator = await GetCreatorAsync(creatorId.Value);
            CreatorHasPermissions(creator);
            return DetermineRoleBasedOnCreator(dto, creator);
        }

        private (RoleEnum role, int createdBy) PublicRegistration(UserRegisterRequestDto dto)
        {
            if (dto.Role != null && dto.Role != RoleEnum.Player)
            {
                throw new ForbiddenException("No puedes crear usuarios con roles específicos sin autenticación.");
            }
            return (RoleEnum.Player, 0); // 0 indica autoregistro
        }

        private async Task<UserResponseDto> GetCreatorAsync(int creatorId)
        {
            var creator = await _userDao.GetUserByIdAsync(creatorId);
            return creator ?? throw new NotFoundException("Usuario creador no encontrado.");
        }

        private void CreatorHasPermissions(UserResponseDto creator)
        {
            bool isAdmin = creator.Role == RoleEnum.Admin;
            bool isOrganizer = creator.Role == RoleEnum.Organizer;

            if (!isAdmin && !isOrganizer)
            {
                throw new ForbiddenException("No tienes permisos para registrar usuarios.");
            }
        }

        private (RoleEnum role, int createdBy) DetermineRoleBasedOnCreator(UserRegisterRequestDto dto, UserResponseDto creator)
        {
            return creator.Role switch
            {
                RoleEnum.Admin => HandleAdminRegistration(dto, creator),
                RoleEnum.Organizer => (RoleEnum.Judge, creator.Id),
                _ => throw new ForbiddenException("Rol no soportado para creación de usuarios.")
            };
        }

        private (RoleEnum role, int createdBy) HandleAdminRegistration(UserRegisterRequestDto dto, UserResponseDto creator)
        {
            if (dto.Role == null)
            {
                throw new ValidationException("El rol es obligatorio para Admins.");
            }
            return (dto.Role.Value, creator.Id);
        }

        private UserRequestDto CreateUser(UserRegisterRequestDto dto, RoleEnum role, int createdBy)
        {
            return new UserRequestDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Alias = dto.Alias,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                CountryCode = dto.CountryCode,
                AvatarUrl = dto.AvatarUrl,
                Role = role,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        private UserResponseDto MapUserResponseDto(UserRequestDto user, int userId)
        {
            return new UserResponseDto
            {
                Id = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Alias = user.Alias,
                Email = user.Email,
                CountryCode = user.CountryCode,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
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

        private async Task ValidateUserDetailsAsync(string alias, string email, string countryCode)
        {
            await ValidateAliasAsync(alias);
            await ValidateEmailAsync(email);
            await _countryService.ValidateCountryAsync(countryCode);
        }

        private async Task<UserResponseDto> ValidateUserExistsAsync(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            return user ?? throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        private void UpdateUserProperties(UserResponseDto user, UserUpdateRequestDto dto)
        {
            user.FirstName = !string.IsNullOrEmpty(dto.FirstName) ? dto.FirstName : user.FirstName;
            user.LastName = !string.IsNullOrEmpty(dto.LastName) ? dto.LastName : user.LastName;
            user.Alias = !string.IsNullOrEmpty(dto.Alias) ? dto.Alias : user.Alias;
            user.Email = !string.IsNullOrEmpty(dto.Email) ? dto.Email : user.Email;
            user.CountryCode = !string.IsNullOrEmpty(dto.CountryCode) ? dto.CountryCode : user.CountryCode;
            user.AvatarUrl = !string.IsNullOrEmpty(dto.AvatarUrl) ? dto.AvatarUrl : user.AvatarUrl;
        }
    }
}
