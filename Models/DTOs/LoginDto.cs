

using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email o Alias es requerido")]
        public string EmailOrAlias { get; set; }

        [Required(ErrorMessage = "Contraseña es requerida")]
        public string Password { get; set; }
    }
}
