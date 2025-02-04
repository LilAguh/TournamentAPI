
namespace Models.DTOs.UserCards
{
    public class UserCardResponseDto
    {
        public int CardId { get; set; }
        public string CardName { get; set; } = string.Empty;
        //Por el momento solo nombre, veremos despues como armamos esto
        public int Quantity { get; set; }
    }
}
