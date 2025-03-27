
using Config;
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

        // Crea una nueva serie y retorna el objeto SeriesResponseDto correspondiente.
        public async Task<SeriesResponseDto> CreateSeriesAsync(SeriesRequestDto dto)
        {
            return await _serieDao.AddSeriesAsync(dto);
        }

        // Retorna todas las series registradas. Lanza una excepción si no hay series.
        public async Task<IEnumerable<SeriesResponseDto>> GetAllSeriesAsync()
        {
            var series = await _serieDao.GetAllSeriesAsync();
            if (!series.Any())
                throw new NotFoundException(ErrorMessages.NoSeriesRegistered);

            return series;
        }

        // Retorna una serie específica por su ID. Lanza una excepción si no se encuentra.
        public async Task<SeriesResponseDto> GetSeriesByIdAsync(int id)
        {
            var series = await _serieDao.GetSeriesByIdAsync(id);
            if (series == null)
                throw new NotFoundException(ErrorMessages.NotFoundSerie);

            return series;
        }

        // Actualiza una serie existente y retorna el objeto actualizado.
        // Lanza una excepción si la serie no existe.
        public async Task<SeriesResponseDto> UpdateSeriesAsync(int id, SeriesRequestDto dto)
        {
            var existingSeries = await _serieDao.GetSeriesByIdAsync(id);
            if (existingSeries == null)
                throw new NotFoundException(ErrorMessages.NotFoundSerie);

            return await _serieDao.UpdateSeriesAsync(id, dto);
        }

        // Elimina una serie. Lanza una excepción si la eliminación falla (serie no encontrada).
        public async Task DeleteSeriesAsync(int id)
        {
            var success = await _serieDao.DeleteSeriesAsync(id);
            if (!success)
                throw new NotFoundException(ErrorMessages.NotFoundSerie);
        }
    }
}
