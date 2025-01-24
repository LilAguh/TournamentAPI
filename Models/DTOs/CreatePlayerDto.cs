

namespace Models.DTOs
{
    public class CreatePlayerDto
    {
            public string Role { get; set; } = "Player";  // Valor predeterminado
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Alias { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public string CountryCode { get; set; }
            public string AvatarUrl { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Valor predeterminado
            public bool IsActive { get; set; } = true;  // Valor predeterminado
            public int CreatedBy { get; set; } = 0;  // Valor predeterminado
        }
    }
