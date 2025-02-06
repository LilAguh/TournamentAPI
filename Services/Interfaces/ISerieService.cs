
using Models.DTOs.Series;

namespace Services.Interfaces
{
    public interface ISerieService
    {
        Task<SeriesResponseDto> CreateSeriesAsync(SeriesRequestDto dto);
        Task<IEnumerable<SeriesResponseDto>> GetAllSeriesAsync();
        Task<SeriesResponseDto?> GetSeriesByIdAsync(int id);
        Task<SeriesResponseDto?> UpdateSeriesAsync(int id, SeriesRequestDto dto);
        Task<bool> DeleteSeriesAsync(int id);
    }
}
