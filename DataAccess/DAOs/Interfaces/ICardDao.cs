using Models.DTOs.Cards;

namespace DataAccess.DAOs.Interfaces
{
    public interface ICardDao
    {
        Task<int> AddCardAsync(CardRequestDto card, int adminId);
        Task<IEnumerable<CardResponseDto>> GetAllCardsAsync();
        Task<CardResponseDto?> GetCardByIdAsync(int id);
    }
}
