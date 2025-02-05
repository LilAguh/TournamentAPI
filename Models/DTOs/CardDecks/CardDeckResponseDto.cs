
namespace Models.DTOs.CardDecks
{
    public class CardDeckResponseDto
    {
        public int DeckId { get; set; }
        public int CardId { get; set; }
        public string CardName { get; set; } = string.Empty;
    }
}
