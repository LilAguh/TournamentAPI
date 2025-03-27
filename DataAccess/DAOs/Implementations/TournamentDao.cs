using Config;
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.Tournament;
using Models.Enums;
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

        // Agrega un nuevo torneo a la base de datos y retorna el ID generado.
        public async Task<int> AddTournamentAsync(TournamentRequestDto dto, int organizerId, int maxPlayers, int maxGames)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"INSERT INTO Tournament (Name, OrganizerID, StartDate, EndDate, CountryCode, MaxPlayers, MaxGames, CountPlayers, Phase)
                          VALUES (@Name, @OrganizerID, @StartDate, @EndDate, @CountryCode, @MaxPlayers, @MaxGames, 0, @Phase)";
            await connection.ExecuteAsync(query, new
            {
                dto.Name,
                OrganizerID = organizerId,
                dto.StartDate,
                dto.EndDate,
                dto.CountryCode,
                MaxPlayers = maxPlayers,
                MaxGames = maxGames,
                Phase = PhaseEnum.Registration.ToString().ToLowerInvariant()
            });
            return await connection.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");
        }

        // Obtiene los detalles de un torneo por su ID.
        public async Task<TournamentResponseDto> GetTournamentByIdAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Tournament WHERE ID = @Id";
            return await connection.QueryFirstOrDefaultAsync<TournamentResponseDto>(query, new { Id = tournamentId });
        }

        // Obtiene todos los torneos registrados en la base de datos.
        public async Task<IEnumerable<TournamentResponseDto>> GetAllTournamentsAsync()
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Tournament";
            return await connection.QueryAsync<TournamentResponseDto>(query);
        }

        // Obtiene un torneo por su fase.
        public async Task<TournamentResponseDto> GetTournamentByPhaseAsync(int tournamentPhase)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Tournament WHERE Phase = @Phase";
            return await connection.QueryFirstOrDefaultAsync<TournamentResponseDto>(query, new { Phase = tournamentPhase });
        }

        // Actualiza la fase de un torneo existente.
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
                throw new NotFoundException(ErrorMessages.TournamentNotFoundForUpdate);
        }

        // Incrementa el contador de jugadores registrados en un torneo.
        public async Task IncrementCountPlayersAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "UPDATE Tournament SET CountPlayers = CountPlayers + 1 WHERE ID = @TournamentId";
            await connection.ExecuteAsync(query, new { TournamentId = tournamentId });
        }

        // Finaliza un torneo y asigna al ganador.
        public async Task FinalizeTournamentAsync(int tournamentId, int winnerId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "UPDATE Tournament SET Phase = 'finalized', WinnerID = @WinnerId WHERE ID = @TournamentId";
            int rowsAffected = await connection.ExecuteAsync(query, new { TournamentId = tournamentId, WinnerId = winnerId });
            if (rowsAffected == 0)
                throw new ValidationException(ErrorMessages.TournamentFinalizeError);
        }

        // Obtiene las series habilitadas para un torneo.
        public async Task<List<int>> GetEnabledSeriesAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @"SELECT SeriesID FROM TournamentSeries WHERE TournamentID = @TournamentId";
            return (await connection.QueryAsync<int>(query, new { TournamentId = tournamentId })).ToList();
        }

        // Agrega una serie permitida a un torneo.
        public async Task AddAllowedSeriesAsync(int tournamentId, int seriesId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "INSERT INTO TournamentSeries (TournamentID, SeriesID) VALUES (@TournamentId, @SeriesId)";
            await connection.ExecuteAsync(query, new { TournamentId = tournamentId, SeriesId = seriesId });
        }

        // Elimina una serie permitida de un torneo.
        public async Task RemoveAllowedSeriesAsync(int tournamentId, int seriesId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "DELETE FROM TournamentSeries WHERE TournamentID = @TournamentId AND SeriesID = @SeriesId";
            await connection.ExecuteAsync(query, new { TournamentId = tournamentId, SeriesId = seriesId });
        }

        // Obtiene las series permitidas para un torneo.
        public async Task<List<int>> GetAllowedSeriesAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT SeriesID FROM TournamentSeries WHERE TournamentID = @TournamentId";
            return (await connection.QueryAsync<int>(query, new { TournamentId = tournamentId })).ToList();
        }
    }
}