
namespace Models.DTOs
{
    public class UserUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
