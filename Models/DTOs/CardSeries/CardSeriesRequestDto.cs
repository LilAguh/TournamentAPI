
using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.CardSeries
{
    public class CardSeriesRequestDto
    {
        [Required(ErrorMessage = ErrorMessages.CardIdRequired)]
        public int CardId { get; set; }

        [Required(ErrorMessage = ErrorMessages.SerieIdRequired)]
        public int SeriesId { get; set; }
    }
}
