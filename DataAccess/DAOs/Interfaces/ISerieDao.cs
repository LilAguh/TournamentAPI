
using Models.DTOs.Series;

namespace DataAccess.DAOs.Interfaces
{
    public interface ISerieDao
    {
        Task<SeriesResponseDto> AddSeriesAsync(SeriesRequestDto dto);
        Task<IEnumerable<SeriesResponseDto>> GetAllSeriesAsync();
        Task<SeriesResponseDto?> GetSeriesByIdAsync(int id);
        Task<SeriesResponseDto?> UpdateSeriesAsync(int id, SeriesRequestDto dto);
        Task<bool> DeleteSeriesAsync(int id);
    }
}
