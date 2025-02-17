
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.Cards;
using Models.DTOs.Tournament;
using Models.DTOs.User;
using Models.Enums;
using System.Diagnostics.Metrics;

namespace DataAccess.DAOs.Implementations
{
    public class TournamentDao : ITournamentDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public TournamentDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<int> AddTournamentAsync(TournamentRequestDto dto, int organizerId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" INSERT INTO Tournaments (Name, OrganizerID, StartDate, EndDate, StartTime, EndTime, Country, Phase)
                           VALUES (@Name, @OrganizerID, @StartDate, @EndDate, @StartTime, @EndTime, @Country, @Phase)
                           SELECT LAST_INSERT_ID()";

            int newTournament = await connection.ExecuteScalarAsync<int>(query,
                new
                {
                    dto.Name,
                    OrganizerID = organizerId,
                    dto.StartDate,
                    dto.EndDate,
                    dto.StartTime,
                    dto.EndTime,
                    dto.Country,
                    Phase = PhaseEnum.Registration
                });
            return newTournament;
        }

        public async Task<TournamentResponseDto> GetTournamentByIdAsync(int tournamentID)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Tournament WHERE ID = @Id";
            return await connection.QueryFirstOrDefaultAsync<TournamentResponseDto>(query, new { Id = tournamentID });
        }

        public async Task<IEnumerable<TournamentResponseDto>> GetAllTournamentsAsync()
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Tournament";
            return await connection.QueryAsync<TournamentResponseDto>(query);
        }

        public async Task<TournamentResponseDto> GetTournamentByPhaseAsync(int tournamentPhase)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Tournament WHERE Phase = @Phase";
            return await connection.QueryFirstOrDefaultAsync<TournamentResponseDto>(query, new { Phase = tournamentPhase });
        }
    }
}
