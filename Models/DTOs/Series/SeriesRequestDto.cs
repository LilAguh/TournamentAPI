
using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Series
{
    public class SeriesRequestDto
    {
        [Required(ErrorMessage = ErrorMessages.SeriesNameIsRequired)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = ErrorMessages.SeriesNameCharacters)]
        public string Name { get; set; }

        [Required(ErrorMessage = ErrorMessages.SeriesDateIsRequired)]
        public DateTime CreatedAt { get; set; }
    }
}
