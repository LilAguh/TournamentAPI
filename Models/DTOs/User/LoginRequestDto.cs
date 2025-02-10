

using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.User
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = ErrorMessages.UserDataRequired)]
        public string EmailOrAlias { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredPassword)]
        public string Password { get; set; }
    }
}
