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
        Task<CardResponseDto> CreateCardAsync(CardRequestDto card, int adminId);
        Task<IEnumerable<CardResponseDto>> GetAllCardsAsync();
        Task<CardResponseDto?> GetCardByIdAsync(int id);
        Task<CardResponseDto> UpdateCardAsync(int id, CardRequestDto card, int adminId);
        Task<bool> DeleteCardAsync(int id, int adminId);
    }
}
