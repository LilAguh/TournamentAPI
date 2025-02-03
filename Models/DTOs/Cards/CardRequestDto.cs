
namespace Models.DTOs.Cards
{
    public class CardRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string IllustrationUrl { get; set; }
    }
}
