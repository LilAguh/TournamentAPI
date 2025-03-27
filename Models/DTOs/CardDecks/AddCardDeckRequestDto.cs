
using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.CardDecks
{
    public class AddCardDeckRequestDto
    {
        [Required(ErrorMessage = ErrorMessages.LeastOneCard)]
        [MinLength(1, ErrorMessage = ErrorMessages.LeastOneCard)]
        [MaxLength(15, ErrorMessage = ErrorMessages.LimitCardSentDeck)]
        public List<int> CardId { get; set; }
    }

}
