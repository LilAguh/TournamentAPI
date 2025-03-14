﻿using Config;
using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.User
{
    public class UserRegisterRequestDto
    {
        [Required(ErrorMessage = ErrorMessages.RequiredName)]
        //[StringLength(50, MinimumLength = 5, ErrorMessage = "El nombre no puede tener menos de 5 caracteres, ni más de 50 caracteres.")]
        [StringLength(50, ErrorMessage = ErrorMessages.NameExceedCharacters)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ErrorMessages.LastNameRequired)]
        [StringLength(50, ErrorMessage = ErrorMessages.LastNameExceedCharacters)]
        public string LastName { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredAlias)]
        [StringLength(30, MinimumLength = 3, ErrorMessage = ErrorMessages.InvalidAlias)]
        public string Alias { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredEmail)]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$", ErrorMessage = ErrorMessages.InvalidEmail)]
        public string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.RequiredPassword)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = ErrorMessages.InvalidPassword)]
        public string Password { get; set; }

        [Required(ErrorMessage = ErrorMessages.CountryCodeRequired)]
        [StringLength(2, MinimumLength = 2, ErrorMessage = ErrorMessages.InvalidCode)]
        public string CountryCode { get; set; }

        [Url(ErrorMessage = ErrorMessages.InvalidAvatarUrl)]
        public string? AvatarUrl { get; set; }

        [Required(ErrorMessage = ErrorMessages.IncorrectRole)]
        [Range(1, 4, ErrorMessage = ErrorMessages.IncorrectRole)]
        public RoleEnum Role { get; set; }
    }
}
