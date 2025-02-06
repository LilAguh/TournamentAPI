
using Models.DTOs.CardSeries;

namespace DataAccess.DAOs.Interfaces
{
    public interface ICardSeriesDao
    {
        Task<bool> AddCardToSeriesAsync(AddCardSeriesRequestDto dto);
        Task<bool> RemoveCardFromSeriesAsync(int cardId, int seriesId);
        Task<IEnumerable<CardSeriesResponseDto>> GetCardsBySeriesAsync(int seriesId);
        Task<IEnumerable<CardSeriesResponseDto>> GetSeriesByCardAsync(int cardId);
    }
}
