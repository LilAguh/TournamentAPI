
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Disqualifications
{
    public class DisqualificationRequestDto
    {
        [Required(ErrorMessage = "El ID del torneo es obligatorio.")]
        public int TournamentID { get; set; }

        [Required(ErrorMessage = "El ID del jugador es obligatorio.")]
        public int PlayerID { get; set; }

        [Required(ErrorMessage = "La razón de la descalificación es obligatoria.")]
        [StringLength(255, ErrorMessage = "La razón no puede exceder 255 caracteres.")]
        public string Reason { get; set; }
    }
}
