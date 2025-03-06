
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Decks
{
    public class DeckRequestDto
    {
        [Required(ErrorMessage = "El nombre del mazo es obligatorio.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El nombre del mazo no puede tener menos 5 y de más de 50 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }
}
