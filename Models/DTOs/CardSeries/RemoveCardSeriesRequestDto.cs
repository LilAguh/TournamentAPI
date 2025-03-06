
using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.CardSeries
{
    public class RemoveCardSeriesRequestDto
    {
        [Required(ErrorMessage = "El ID de la carta es obligatorio.")]
        public int CardId { get; set; }

        [Required(ErrorMessage = "El ID de la serie es obligatorio.")]
        public int SeriesId { get; set; }
    }
}
