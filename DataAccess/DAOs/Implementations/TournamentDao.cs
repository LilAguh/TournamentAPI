
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.Cards;
using Models.DTOs.Tournament;
using Models.DTOs.User;
using Models.Enums;
using Models.Exceptions;
using System.Diagnostics.Metrics;
using static Models.Exceptions.CustomException;

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

            int maxPlayers = await CalculateMaxPlayersAsync(dto);
            int maxGames = maxPlayers - 1;

            var query = @"INSERT INTO Tournament (Name, OrganizerID, StartDate, EndDate, StartTime, EndTime, CountryCode, MaxPlayers, MaxGames, CountPlayers, Phase)
                          VALUES (@Name, @OrganizerID, @StartDate, @EndDate, @StartTime, @EndTime, @CountryCode, @MaxPlayers, @MaxGames, 0, @Phase)";

            await connection.ExecuteAsync(query, new
            {
                dto.Name,
                OrganizerID = organizerId,
                dto.StartDate,
                dto.EndDate,
                dto.StartTime,
                dto.EndTime,
                dto.CountryCode,
                MaxPlayers = maxPlayers,
                MaxGames = maxGames,
                Phase = PhaseEnum.Registration.ToString().ToLowerInvariant()
            });

            return await connection.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");
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

        public async Task<int> CalculateMaxPlayersAsync(TournamentRequestDto dto)
        { 
            int dayAvailable = 1 + (dto.EndDate - dto.StartDate).Days;

            int minutesDay = (int)(dto.EndTime - dto.StartTime).TotalMinutes;

            int totalMatches = (dayAvailable * minutesDay) / 30;

            int maxPlayers = 2;
            int matches = 1;
            while (matches * 2 - 1 <= totalMatches)
            {
                maxPlayers *= 2;
                matches = maxPlayers - 1;
            }

            return maxPlayers;
        }

        public async Task UpdateTournamentPhaseAsync(int tournamentId, string newPhase)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"UPDATE Tournament
                          SET Phase = @NewPhase
                          WHERE ID = @TournamentId";

            int rowsAffected = await connection.ExecuteAsync(query, new
            {
                TournamentId = tournamentId,
                NewPhase = newPhase
            });
            if (rowsAffected == 0)
            {
                throw new NotFoundException("No se encontró el torneo para actualizar su fase");
            }
        }

        public async Task IncrementCountPlayersAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "UPDATE Tournament SET CountPlayers = CountPlayers + 1 WHERE ID = @TournamentId";
            await connection.ExecuteAsync(query, new { TournamentId = tournamentId });
        }

        public async Task FinalizeTournamentAsync(int tournamentId, int winnerId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "UPDATE Tournament SET Phase = 'finalized', WinnerID = @WinnerId WHERE ID = @TournamentId";
            int rowsAffected = await connection.ExecuteAsync(query, new { TournamentId = tournamentId, WinnerId = winnerId });
            if (rowsAffected == 0)
                throw new ValidationException("No se pudo finalizar el torneo");
        }

        public async Task<List<int>> GetEnabledSeriesAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT SeriesID FROM TournamentSeries WHERE TournamentID = @TournamentId";
            return (await connection.QueryAsync<int>(query, new { TournamentId = tournamentId })).ToList();
        }
    }
}
