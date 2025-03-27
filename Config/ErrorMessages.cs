

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
        public const string InvalidCredentialsPassword = "Password are invalid.";

        //userservice:
        public const string DataUserAlreadyUse = "The email or alias is already in use.";
        public const string AccesDenied = "Acces is denied.";
        public const string AccountDeactivated = "Account deactivated.";
        public const string IdDiffer = "User ID and token ID differ, this account cannot be modified.";

        //admincontroller:
        public const string UserNotFound = "User not found.";
        public const string InternalServerError = "Internal server error.";
        public const string AdminTokenNotFound = "Admin ID not found in token.";
        public const string InvalidFormatTokenId = "Invalid format for administrator ID.";

        //Country:
        public const string InvalidCountryCode = "Invalid country code.";
        public const string NoCountriesRegistered = "There are no countries registered.";

        //UserCard
        public const string CardNotFound = "The card has not been found.";
        public const string NoCardsRegistered = "There are no registered cards.";
        public const string CardDeleted = "Card successfully deleted.";
        public const string CardAlreadyExist = "There is already a card with this name.";
        public const string CardIdRequired = "Card ID is required.";
        public const string AmountRequired = "The amount is required.";
        public const string ErrorQuantity = "The quantity must be between 1 and 100.";
        public const string CardAddCollection = "Card added to your collection.";
        public const string CardRemovedCollection = "Card removed from your collection.";

        //Decks
        public const string ErrorCreateDeck = "Error creating deck.";
        public const string NoRegisteredDecks = "You have no registered decks.";
        public const string NotFoundDeck = "Deck not found.";
        public const string ErrorDeleteDeck = "You cannot delete this deck.";
        public const string DeleteDeck = "Deck has been removed.";

        //UserService:
        public const string AliasAlreadyUse = "Alias is already in use.";
        public const string EmailAlreadyUse = "The email is already in use by an active user.";
        public const string CannotCreateUsers = "You cannot create users with specific roles without authentication.";
        public const string CreatorUserNotFound = "Creator user not found.";
        public const string NotHavePermissionRegister = "You do not have permission to register users.";
        public const string RoleNoSupportedUser = "Role not supported for user creation.";
        public const string AdminRoleRequired = "The role is required for Admins.";
        public const string AdminInvalid = "The admin is not valid.";

        //UserController
        public const string PasswordUpdated = "Password updated successfully.";
        public const string GetUserIdException = "Unauthenticated user.";
        public const string AccountDeleted = "User has been deleted.";
    }
}
