
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.TournamentPlayers
{
    public class TournamentPlayerRegistrationDto
    {
        [Required]
        public int TournamentID { get; set; }

        [Required]
        public int DeckId { get; set; }
    }
}
