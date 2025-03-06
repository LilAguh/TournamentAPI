
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.CardDecks
{
    public class AddCardDeckRequestDto
    {
        [Required(ErrorMessage = "Debe enviarse al menos una carta.")]
        [Range(1, 15, ErrorMessage = "Solamente se pueden enviar entre 1 y 15 cartas a cada mazo.")]
        public List<int> CardId { get; set; }
    }

}
