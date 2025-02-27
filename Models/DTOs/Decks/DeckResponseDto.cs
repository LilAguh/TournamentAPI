
namespace Models.DTOs.Decks
{
    public class DeckResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CardCount { get; set; }
        // podría incluir una lista de cartas (llamada por el módulo CardDecks)
    }
}
