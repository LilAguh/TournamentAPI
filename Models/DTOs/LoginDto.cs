

using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = ErrorMessages.UserDataRequired)]
        public string EmailOrAlias { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredPassword)]
        public string Password { get; set; }
    }
}
