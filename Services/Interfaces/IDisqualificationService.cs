
using Models.DTOs.Disqualifications;

namespace Services.Interfaces
{
    public interface IDisqualificationService
    {
        Task<int> DisqualifyPlayerAsync(DisqualificationRequestDto requestDto, int judgeId);
    }
}
