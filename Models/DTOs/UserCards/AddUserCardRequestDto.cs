
using Config;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.UserCards
{
    public class AddUserCardRequestDto
    {
        [Required(ErrorMessage = "El ID de la carta es obligatorio.")]
        public int CardId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        public int Quantity { get; set; } = 1;
    }
}
