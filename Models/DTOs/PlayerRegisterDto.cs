
namespace Models.DTOs
{
    public class PlayerRegisterDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public string? Avatar { get; set; }
    }
}
