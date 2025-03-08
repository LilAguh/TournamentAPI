
namespace Config
{
    public class ErrorMessages
    {
        //userdto:
        public const string RequiredName = "Name is required.";
        public const string NameExceedCharacters = "Name cannot exceed 50 characters.";
        public const string LastNameRequired = "Last name is required.";
        public const string LastNameExceedCharacters = "Last name cannot exceed 50 characters.";
        public const string RequiredAlias = "Alias is required.";
        public const string InvalidAlias = "Alias must be 3 to 30 characters long.";
        public const string RequiredEmail = "Email is required.";
        public const string InvalidEmail = "Email format is invalid. Must be: User@Domain.com.";
        public const string RequiredPassword = "Password is required.";
        public const string InvalidPassword = "Password must be at least 8 Characters, 1 uppercase, 1 lowercase and 1 number.";
        public const string CountryCodeRequired = "Country code is required.";
        public const string InvalidCode = "Code must be 2 characters long.";
        public const string InvalidAvatarUrl = "Avatar url is invalid.";
        public const string IncorrectRole = "Role is incorrect.";
        public const string InvalidRole = "Rol must be between 1 and 4.";

        //logindto:
        public const string UserDataRequired = "Email or Alias ​​is required.";

        //jwtHelper:
        public const string UnconfiguredSecretKey = "SecretKey not configured.";
        public const string UnconfiguredIssuer = "Issuer not configured.";

        //authservice:
        public const string InvalidCredentials = "Credentials are invalid.";

        //userservice:
        public const string DataUserAlreadyUse = "The email or alias is already in use.";
        public const string AccesDenied = "Acces is denied.";
        public const string AccountDeactivated = "Account deactivated.";
        public const string IdDiffer = "User ID and token ID differ, this account cannot be modified.";

        //admincontroller:
        public const string UserNotFound = "User not found.";
        public const string InternalServerError = "Internal server error.";

        //Country:
        public const string InvalidCountryCode = "Código de país inválido.";

        //UserCard
        //public const string
        public const string CardNotFound = "Carta no encontrada.";

        //UserService:
        public const string AliasAlreadyUse = "Alias is already in use.";
    }
}
