
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.TournamentPlayers
{
    public class TournamentPlayerRegistrationDto
    {
        [Required(ErrorMessage = "El ID del torneo es obligatorio.")]
        public int TournamentID { get; set; }

        [Required(ErrorMessage = "El ID del mazo es obligatorio.")]
        public int DeckId { get; set; }
    }
}
