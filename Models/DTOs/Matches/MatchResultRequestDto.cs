
using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Matches
{
    public class MatchResultRequestDto
    {
        [Required(ErrorMessage = "El ID del partido es obligatorio.")]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "El ID del ganador es obligatorio.")]
        public int WinnerId { get; set; }
    }
}
