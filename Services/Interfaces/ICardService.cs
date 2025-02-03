using Models.DTOs.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICardService
    {
        Task<int> CreateCardAsync(CardRequestDto card, int adminId);
        Task<IEnumerable<CardResponseDto>> GetAllCardsAsync();
        Task<CardResponseDto?> GetCardByIdAsync(int id);
        Task<bool> UpdateCardAsync(int id, CardRequestDto card, int adminId);
    }
}
