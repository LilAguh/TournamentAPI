using DataAccess.DAOs.Interfaces;
using Models.DTOs.User;
using Services.Helpers;
using Models.Enums;
using Config;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    // Servicio para gestionar las operaciones relacionadas con usuarios.
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly PasswordHasher _passwordHasher;
        private readonly ICountryService _countryService;

        // Constructor del servicio de usuario
        public UserService(
            IUserDao userDao,
            PasswordHasher passwordHasher,
            ICountryService countryService)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _countryService = countryService;
        }

        // Registra un nuevo usuario en el sistema.
        public async Task<UserResponseDto> Register(UserRegisterRequestDto dto, int? creatorId)
        {
            await ValidateUserDetailsAsync(dto.Alias, dto.Email, dto.CountryCode);
            var (role, createdBy) = await DetermineRoleAndCreator(dto, creatorId);

            var user = CreateUser(dto, role, createdBy);
            var userId = await _userDao.AddUserAsync(user);

            return MapUserResponseDto(user, userId);
        }

        // Actualiza los datos de un usuario existente.
        public async Task<UserResponseDto> UpdateUser(int id, UserUpdateRequestDto dto)
        {
            var user = await ValidateUserExistsAsync(id);

            if (!string.IsNullOrEmpty(dto.CountryCode))
                await _countryService.ValidateCountryAsync(dto.CountryCode);

            UpdateUserProperties(user, dto);
            await _userDao.UpdateUserAsync(user);
            return user;
        }

        // Cambia la contraseña de un usuario.
        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto dto)
        {
            var user = await ValidateUserExistsAsync(userId);
            _passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash);

            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            await _userDao.UpdateUserAsync(user);
        }

        // Desactiva un usuario (eliminación lógica).
        public async Task DeleteUser(int id)
        {
            var user = await ValidateUserExistsAsync(id);
            user.IsActive = false;
            await _userDao.UpdateUserStatusAsync(user);
        }

        // Obtiene los datos de un usuario por su ID.
        public async Task<UserResponseDto> GetUserById(int id)
        {
            var user = await ValidateUserExistsAsync(id);
            return user;
        }

        // Elimina permanentemente un usuario de la base de datos.
        public async Task DeletePermanentUser(int id)
        {
            var user = await ValidateUserExistsAsync(id);
            await _userDao.PermanentDeleteUserAsync(id);
        }

        // Métodos privados //

        // Determina el rol y el creador de un nuevo usuario.
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

        // Maneja el registro público de usuarios (sin autenticación).
        private (RoleEnum role, int createdBy) PublicRegistration(UserRegisterRequestDto dto)
        {
            if (dto.Role != null && dto.Role != RoleEnum.Player)
            {
                throw new ForbiddenException("No puedes crear usuarios con roles específicos sin autenticación.");
            }
            return (RoleEnum.Player, 0);
        }

        // Obtiene el usuario creador desde la base de datos.
        private async Task<UserResponseDto> GetCreatorAsync(int creatorId)
        {
            var creator = await _userDao.GetUserByIdAsync(creatorId);
            return creator ?? throw new NotFoundException("Usuario creador no encontrado.");
        }

        // Valida que el creador tenga permisos para registrar usuarios.
        private void CreatorHasPermissions(UserResponseDto creator)
        {
            bool isAdmin = creator.Role == RoleEnum.Admin;
            bool isOrganizer = creator.Role == RoleEnum.Organizer;

            if (!isAdmin && !isOrganizer)
            {
                throw new ForbiddenException("No tienes permisos para registrar usuarios.");
            }
        }

        // Determina el rol del nuevo usuario basado en el rol del creador.
        private (RoleEnum role, int createdBy) DetermineRoleBasedOnCreator(UserRegisterRequestDto dto, UserResponseDto creator)
        {
            if (creator.Role == RoleEnum.Admin)
            {
                return HandleAdminRegistration(dto, creator);
            }
            else if (creator.Role == RoleEnum.Organizer)
            {
                return (RoleEnum.Judge, creator.Id);
            }
            else
            {
                throw new ForbiddenException("Rol no soportado para creación de usuarios.");
            }
        }

        // Maneja el registro de usuarios por un Admin.
        private (RoleEnum role, int createdBy) HandleAdminRegistration(UserRegisterRequestDto dto, UserResponseDto creator)
        {
            if (dto.Role == null)
            {
                throw new ValidationException("El rol es obligatorio para Admins.");
            }
            return (dto.Role.Value, creator.Id);
        }

        // Crea un objeto UserRequestDto a partir de un UserRegisterRequestDto.
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

        // Mapea un UserRequestDto a un UserResponseDto.
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

        // Valida que el alias no esté en uso.
        private async Task ValidateAliasAsync(string alias)
        {
            var existingAlias = await _userDao.GetUserByAliasAsync(alias) != null ?
                throw new ValidationException(ErrorMessages.AliasAlreadyUse)
                : true;
        }

        // Valida que el email no esté en uso.
        private async Task ValidateEmailAsync(string email)
        {
            var existingEmailUser = await _userDao.GetActiveUserByEmailAsync(email) != null ?
                throw new ValidationException(ErrorMessages.EmailAlreadyUse)
                : true;
        }

        // Valida los detalles del usuario (alias, email y código de país).
        private async Task ValidateUserDetailsAsync(string alias, string email, string countryCode)
        {
            await ValidateAliasAsync(alias);
            await ValidateEmailAsync(email);
            await _countryService.ValidateCountryAsync(countryCode);
        }

        // Valida que un usuario exista en la base de datos.
        private async Task<UserResponseDto> ValidateUserExistsAsync(int id)
        {
            var user = await _userDao.GetUserByIdAsync(id);
            return user ?? throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        // Actualiza las propiedades de un usuario con los datos proporcionados.
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