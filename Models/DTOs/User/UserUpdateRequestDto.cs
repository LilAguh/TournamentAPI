using Config;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.User
{
    public class UserUpdateRequestDto
    {
        [StringLength(50, ErrorMessage = ErrorMessages.NameExceedCharacters)]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = ErrorMessages.LastNameExceedCharacters)]
        public string? LastName { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = ErrorMessages.InvalidAlias)]
        public string? Alias { get; set; }

        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$", ErrorMessage = ErrorMessages.InvalidEmail)]
        public string? Email { get; set; }

        [StringLength(2, MinimumLength = 2, ErrorMessage = ErrorMessages.InvalidCode)]
        public string? CountryCode { get; set; }

        [Url(ErrorMessage = ErrorMessages.InvalidAvatarUrl)]
        public string? AvatarUrl { get; set; }
    }
}
