
using Models.Enums;

namespace Models.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string CountryCode { get; set; }
        public string AvatarUrl { get; set; }
        public RoleEnum Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
