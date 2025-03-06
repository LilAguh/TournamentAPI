

using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Tournament
{
    public class TournamentRequestDto
    {
        [Required(ErrorMessage = "El nombre del torneo es obligatorio.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El nombre del torneo no puede tener menos de 5 caracteres, ni más de 50 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es obligatoria.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "El código del país es obligatorio.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "El código de país debe tener exactamente 2 caracteres.")]
        public string CountryCode { get; set; }
    }
}
