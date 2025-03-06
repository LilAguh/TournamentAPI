
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Series
{
    public class SeriesRequestDto
    {
        [Required(ErrorMessage = "El nombre de la serie es obligatorio.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El nombre de la serie no puede tener menos de 5 caracteres, ni más de 50 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La fecha de creación es obligatoria.")]
        public DateTime CreatedAt { get; set; }
    }
}
