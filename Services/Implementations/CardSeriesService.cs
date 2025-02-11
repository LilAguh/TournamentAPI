
using DataAccess.DAOs.Interfaces;
using Models.DTOs.CardSeries;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class CardSeriesService : ICardSeriesService
    {
        private readonly ICardSeriesDao _cardSeriesDao;

        public CardSeriesService(ICardSeriesDao cardSeriesDao)
        {
            _cardSeriesDao = cardSeriesDao;
        }

        public async Task AddCardToSeriesAsync(AddCardSeriesRequestDto dto)
        {
            var success = await _cardSeriesDao.AddCardToSeriesAsync(dto);
            if (!success)
                throw new ValidationException("La carta ya pertenece a esta serie.");
        }

        public async Task RemoveCardFromSeriesAsync(int cardId, int seriesId)
        {
            var success = await _cardSeriesDao.RemoveCardFromSeriesAsync(cardId, seriesId);
            if (!success)
                throw new NotFoundException("Relación carta-serie no encontrada.");
        }

        public async Task<IEnumerable<CardSeriesResponseDto>> GetCardsBySeriesAsync(int seriesId)
        {
            var cards = await _cardSeriesDao.GetCardsBySeriesAsync(seriesId);
            if (!cards.Any())
                throw new NotFoundException("No hay cartas en esta serie.");

            return cards;
        }

        public async Task<IEnumerable<CardSeriesResponseDto>> GetSeriesByCardAsync(int cardId)
        {
            var series = await _cardSeriesDao.GetSeriesByCardAsync(cardId);
            if (!series.Any())
                throw new NotFoundException("Esta carta no pertenece a ninguna serie.");

            return series;
        }
    }
}
