
using DataAccess.DAOs.Implementations;
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Series;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class SerieService : ISerieService
    {
        private readonly ISerieDao _serieDao;

        public SerieService(ISerieDao serieDao)
        {
            _serieDao = serieDao;
        }

        public async Task<SeriesResponseDto> CreateSeriesAsync(SeriesRequestDto dto)
        {
            return await _serieDao.AddSeriesAsync(dto);
        }

        public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesAsync()
        {
            var series = await _serieDao.GetAllSeriesAsync();
            if (!series.Any())
                throw new NotFoundException("No hay series registradas.");

            return series;
        }

        public async Task<SeriesResponseDto> GetSeriesByIdAsync(int id)
        {
            var series = await _serieDao.GetSeriesByIdAsync(id);
            if (series == null)
                throw new NotFoundException("Serie no encontrada.");

            return series;
        }

        public async Task<SeriesResponseDto> UpdateSeriesAsync(int id, SeriesRequestDto dto)
        {
            var existingSeries = await _serieDao.GetSeriesByIdAsync(id);
            if (existingSeries == null)
                throw new NotFoundException("Serie no encontrada.");

            return await _serieDao.UpdateSeriesAsync(id, dto);
        }

        public async Task DeleteSeriesAsync(int id)
        {
            var success = await _serieDao.DeleteSeriesAsync(id);
            if (!success)
                throw new NotFoundException("Serie no encontrada.");
        }
    }
}
