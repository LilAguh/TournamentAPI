
namespace Services.Interfaces
{
    public interface IMatchService
    {
        Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds);
    }
}
