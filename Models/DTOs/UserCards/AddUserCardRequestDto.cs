
namespace Models.DTOs.UserCards
{
    public class AddUserCardRequestDto
    {
        public int CardId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
