
namespace Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Role { get; set; } = "Player";  // Default role is "Player"
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CountryCode { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default to current UTC time
        public DateTime? LastLogin { get; set; }  // Nullable for initial creation
        public bool IsActive { get; set; } = true;  // Default to active
        public int CreatedBy { get; set; } = 0;  // Default to 0 (not created by anyone)
    }
}
