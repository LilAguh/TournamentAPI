
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Series;
using Services.Interfaces;

namespace Services.Implementations
{
    public class SerieService : ISerieService
    {
        private readonly ISerieDao _seriesDao;

        public SerieService(ISerieDao seriesDao)
        {
            _seriesDao = seriesDao;
        }

        public async Task<SeriesResponseDto> CreateSeriesAsync(SeriesRequestDto dto)
        {
            return await _seriesDao.AddSeriesAsync(dto);
        }

        public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesAsync()
        {
            return await _seriesDao.GetAllSeriesAsync();
        }

        public async Task<SeriesResponseDto?> GetSeriesByIdAsync(int id)
        {
            return await _seriesDao.GetSeriesByIdAsync(id);
        }

        public async Task<SeriesResponseDto?> UpdateSeriesAsync(int id, SeriesRequestDto dto)
        {
            return await _seriesDao.UpdateSeriesAsync(id, dto);
        }

        public async Task<bool> DeleteSeriesAsync(int id)
        {
            return await _seriesDao.DeleteSeriesAsync(id);
        }
    }
}
