
using Config;
using Models.Entities;
using static Models.Exceptions.CustomException;

namespace Services.Helpers
{
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var passwordCheck = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            if (!passwordCheck)
                throw new UnauthorizedException(ErrorMessages.InvalidCredentialsPassword);

            return passwordCheck;
        }
    }
}
