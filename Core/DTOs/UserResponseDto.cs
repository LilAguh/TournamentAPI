
namespace Core.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
