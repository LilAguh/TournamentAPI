
namespace Models.DTOs
{
    public class UserResponseDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public bool Active { get; set; } = true;
        public int CreatedBy { get; set; }
    }
}
