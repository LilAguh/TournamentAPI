
using Config;
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

        // Agrega una carta a una serie.
        // Lanza una excepción de validación si la carta ya pertenece a la serie.
        public async Task AddCardToSeriesAsync(CardSeriesRequestDto dto)
        {
            var success = await _cardSeriesDao.AddCardToSeriesAsync(dto);
            if (!success)
                throw new ValidationException(ErrorMessages.CardBelongsSeries);
        }

        // Elimina una carta de una serie.
        // Lanza una excepción NotFound si la relación carta-serie no existe.
        public async Task RemoveCardFromSeriesAsync(int cardId, int seriesId)
        {
            var success = await _cardSeriesDao.RemoveCardFromSeriesAsync(cardId, seriesId);
            if (!success)
                throw new NotFoundException(ErrorMessages.CardSeriesRelationshipError);
        }

        // Obtiene todas las cartas de una serie.
        // Lanza una excepción NotFound si la serie no tiene cartas asociadas.
        public async Task<IEnumerable<CardSeriesResponseDto>> GetCardsBySeriesAsync(int seriesId)
        {
            var cards = await _cardSeriesDao.GetCardsBySeriesAsync(seriesId);
            if (!cards.Any())
                throw new NotFoundException(ErrorMessages.NoCardSeriesError);

            return cards;
        }

        // Obtiene todas las series a las que pertenece una carta.
        // Lanza una excepción NotFound si la carta no pertenece a ninguna serie.
        public async Task<IEnumerable<CardSeriesResponseDto>> GetSeriesByCardAsync(int cardId)
        {
            var series = await _cardSeriesDao.GetSeriesByCardAsync(cardId);
            if (!series.Any())
                throw new NotFoundException(ErrorMessages.CardNotBelongSeries);

            return series;
        }
    }
}
