
using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.UserCards
{
    public class AddUserCardRequestDto
    {
        [Required(ErrorMessage = ErrorMessages.CardIdRequired)]
        public int CardId { get; set; }

        [Required(ErrorMessage = ErrorMessages.AmountRequired)]
        [Range(1, 100, ErrorMessage = ErrorMessages.ErrorQuantity)]
        public int Quantity { get; set; } = 1;
    }
}
