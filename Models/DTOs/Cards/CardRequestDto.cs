
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Cards
{
    public class CardRequestDto
    {
        [Required(ErrorMessage = "El nombre de la carta es obligatorio.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El nombre no puede tener menos 5 y de más de 50 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El ataque es obligatorio.")]
        [Range(0, 9999, ErrorMessage = "El valor de ataque debe estar entre 0 y 9999.")]
        public int Attack { get; set; }

        [Required(ErrorMessage = "La defensa es obligatoria.")]
        [Range(0, 9999, ErrorMessage = "El valor de defensa debe estar entre 0 y 9999.")]
        public int Defense { get; set; }

        [Required(ErrorMessage = "La URL de la ilustración es obligatoria.")]
        [Url(ErrorMessage = "La URL de la ilustración debe ser válida.")]
        [MaxLength(255, ErrorMessage = "La URL no puede tener más de 255 caracteres.")]
        public string IllustrationUrl { get; set; }
    }
}
