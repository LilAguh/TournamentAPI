
using DataAccess.DAOs.Interfaces;
using Models.DTOs.CardSeries;

namespace Services.Interfaces
{
    public interface ICardSeriesService
    {
        Task<bool> AddCardToSeriesAsync(AddCardSeriesRequestDto dto);
        Task<bool> RemoveCardFromSeriesAsync(int cardId, int seriesId);
        Task<IEnumerable<CardSeriesResponseDto>> GetCardsBySeriesAsync(int seriesId);
        Task<IEnumerable<CardSeriesResponseDto>> GetSeriesByCardAsync(int cardId);
    }
}
