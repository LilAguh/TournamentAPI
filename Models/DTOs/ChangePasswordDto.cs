using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = ErrorMessages.RequiredPassword)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredPassword)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = ErrorMessages.InvalidPassword)]
        public string NewPassword { get; set; }
    }
}
