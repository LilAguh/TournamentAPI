
namespace Services.Helpers
{
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "La contraseña no puede ser nula o vacía.");

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "La contraseña no puede ser nula o vacía.");

            if (string.IsNullOrEmpty(hashedPassword))
                throw new ArgumentNullException(nameof(hashedPassword), "El hash de la contraseña no puede ser nulo o vacío.");

            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
