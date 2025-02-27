
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.TournamentJudges
{
    public class TournamentJudgeRequestDto
    {
        [Required(ErrorMessage = "El ID del torneo es obligatorio.")]
        public int TournamentID { get; set; }

        [Required(ErrorMessage = "El ID del juez es obligatorio.")]
        public int JudgeID { get; set; }
    }
}
