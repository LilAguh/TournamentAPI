
using DataAccess.DAOs.Interfaces;
using Models.DTOs.CardSeries;

namespace Services.Implementations
{
    public class CardSeriesService
    {
        private readonly ICardSeriesDao _cardSeriesDao;

        public CardSeriesService(ICardSeriesDao cardSeriesDao)
        {
            _cardSeriesDao = cardSeriesDao;
        }

        public async Task<bool> AddCardToSeriesAsync(AddCardSeriesRequestDto dto)
        {
            return await _cardSeriesDao.AddCardToSeriesAsync(dto);
        }

        public async Task<bool> RemoveCardFromSeriesAsync(int cardId, int seriesId)
        {
            return await _cardSeriesDao.RemoveCardFromSeriesAsync(cardId, seriesId);
        }

        public async Task<IEnumerable<CardSeriesResponseDto>> GetCardsBySeriesAsync(int seriesId)
        {
            return await _cardSeriesDao.GetCardsBySeriesAsync(seriesId);
        }

        public async Task<IEnumerable<CardSeriesResponseDto>> GetSeriesByCardAsync(int cardId)
        {
            return await _cardSeriesDao.GetSeriesByCardAsync(cardId);
        }
    }
}
