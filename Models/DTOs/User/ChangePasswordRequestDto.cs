using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.User
{
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = ErrorMessages.RequiredPassword)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredPassword)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = ErrorMessages.InvalidPassword)]
        public string NewPassword { get; set; }
    }
}
