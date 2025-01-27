﻿

using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class PlayerRegisterDto
    {
        //[Required(ErrorMessage = "El nombre es obligatorio")]
        //[StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
        //public string FirstName { get; set; }

        //[Required(ErrorMessage = "El apellido es obligatorio")]
        //[StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
        //public string LastName { get; set; }

        //[Required(ErrorMessage = "El alias es obligatorio")]
        //[StringLength(30, MinimumLength = 3, ErrorMessage = "El alias debe tener entre 3 y 30 caracteres")]
        //public string Alias { get; set; }

        //[Required(ErrorMessage = "El email es obligatorio")]
        //[RegularExpression(
        //@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$",
        //ErrorMessage = "Formato de email inválido. Debe ser: usuario@dominio.com"
        //)]
        //public string Email { get; set; }

        //[Required(ErrorMessage = "La contraseña es obligatoria")]
        //[RegularExpression(
        //    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        //    ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número"
        //)]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "El código de país es obligatorio")]
        //[StringLength(2, MinimumLength = 2, ErrorMessage = "El código debe tener 2 caracteres")]
        //public string CountryCode { get; set; }

        //[Url(ErrorMessage = "La URL del avatar no es válida")]
        //public string? AvatarUrl { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Alias { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; }

        public string AvatarUrl { get; set; }
    }
}
