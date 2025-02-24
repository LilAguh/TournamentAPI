
using Models.Enums;

namespace Models.DTOs.User
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public UserResponseDto User { get; set; }
    }
}
