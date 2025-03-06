
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using Models.DTOs.Matches;
using Models.Exceptions;
using static Models.Exceptions.CustomException;

namespace DataAccess.DAOs.Implementations
{
    public class MatchDao : IMatchDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public MatchDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds, int round)
        {
            if (playerIds == null)
                throw new Exception("La lista de jugadores no puede ser nula");

            // Se eliminan duplicados para evitar que un jugador se repita
            playerIds = playerIds.Distinct().ToList();

            // Validar que la cantidad de jugadores sea par para formar los partidos
            if (playerIds.Count % 2 != 0)
                throw new Exception("La cantidad de jugadores debe ser par para formar los partidos");

            using var connection = await _databaseConnection.GetConnectionAsync();

            // Consultamos los datos del torneo (para obtener el horario de inicio y MaxPlayers)
            var tournamentQuery = "SELECT StartDate, EndDate, MaxPlayers FROM Tournament WHERE ID = @TournamentId";
            var tournament = await connection.QueryFirstOrDefaultAsync<dynamic>(tournamentQuery, new { TournamentId = tournamentId });
            if (tournament == null)
                throw new NotFoundException("Torneo no encontrado");

            // Verificar que StartDate y EndDate estén definidos
            if (tournament.StartDate == null)
                throw new NotFoundException("StartDate no definido en el torneo");
            if (tournament.EndDate == null)
                throw new NotFoundException("EndDate no definido en el torneo");

            // Programar el MatchStartTime combinando la fecha de inicio y la hora de inicio
            DateTime tournamentStartDate = tournament.StartDate;
            TimeSpan tournamentStartTime = tournamentStartDate.TimeOfDay;
            TimeSpan tournamentEndTime = ((DateTime)tournament.EndDate).TimeOfDay;

            DateTime currentMatchTime;
            int matchNumber;

            // Si no es la primera ronda, obtenemos el horario del último partido de la ronda anterior
            if (round != tournament.MaxPlayers / 2) // Si no es la primera ronda
            {
                var lastMatchQuery = @"SELECT MatchNumber, TotalMatches, MatchStartTime 
                               FROM Matches 
                               WHERE TournamentID = @TournamentId 
                               AND Round = @PreviousRound 
                               ORDER BY MatchStartTime DESC 
                               LIMIT 1";
                var lastMatch = await connection.QueryFirstOrDefaultAsync<(int MatchNumber, int TotalMatches, DateTime MatchStartTime)?>(lastMatchQuery,
                                                new { TournamentId = tournamentId, PreviousRound = round * 2 });

                if (lastMatch.HasValue)
                {
                    // Calcular el horario de inicio del siguiente partido (suponiendo que cada partido dura 30 minutos)
                    DateTime lastMatchEndTime = lastMatch.Value.MatchStartTime.AddMinutes(30);

                    // Si el último partido termina después del horario de finalización, comenzamos al día siguiente
                    if (lastMatchEndTime.TimeOfDay >= tournamentEndTime)
                    {
                        currentMatchTime = lastMatchEndTime.Date.AddDays(1) + tournamentStartTime;
                    }
                    else
                    {
                        currentMatchTime = lastMatchEndTime; // El siguiente partido comienza inmediatamente después
                    }

                    // Continuar el MatchNumber desde el último partido
                    matchNumber = lastMatch.Value.MatchNumber + 1;
                }
                else
                {
                    // Si no hay partidos anteriores, comenzamos desde el inicio del torneo
                    currentMatchTime = tournamentStartDate.Date + tournamentStartTime;
                    matchNumber = 1;
                }
            }
            else
            {
                // Primera ronda: comenzamos desde el inicio del torneo
                currentMatchTime = tournamentStartDate.Date + tournamentStartTime;
                matchNumber = 1;
            }

            // Se calcula el total de partidos del bracket: TotalMatches = MaxPlayers - 1
            int totalMatches = (int)(tournament.MaxPlayers) - 1;

            // Para rondas que no son la primera, obtenemos los ganadores de la ronda anterior
            if (round != tournament.MaxPlayers / 2)
            {
                var winnersQuery = @"SELECT WinnerID 
                             FROM Matches 
                             WHERE TournamentID = @TournamentId 
                             AND Round = @PreviousRound 
                             AND Status = 'Finished' 
                             ORDER BY MatchNumber";
                var winners = await connection.QueryAsync<int>(winnersQuery, new { TournamentId = tournamentId, PreviousRound = round * 2 });
                playerIds = winners.ToList();
            }

            // Mezclar aleatoriamente los IDs de los jugadores sólo en la primera ronda
            if (round == tournament.MaxPlayers / 2)
            {
                playerIds = playerIds.OrderBy(x => Guid.NewGuid()).ToList();
            }

            var insertQuery = @"INSERT INTO Matches (TournamentID, MatchNumber, TotalMatches, Player1ID, Player2ID, MatchStartTime, Status, Round)
                        VALUES (@TournamentID, @MatchNumber, @TotalMatches, @Player1ID, @Player2ID, @MatchStartTime, @Status, @Round)";

            // Recorrer la lista de jugadores emparejándolos en partidos
            for (int i = 0; i < playerIds.Count; i += 2)
            {
                int player1Id = playerIds[i];
                int player2Id = playerIds[i + 1];

                await connection.ExecuteAsync(insertQuery, new
                {
                    TournamentID = tournamentId,
                    MatchNumber = matchNumber,
                    TotalMatches = totalMatches,
                    Player1ID = player1Id,
                    Player2ID = player2Id,
                    MatchStartTime = currentMatchTime,
                    Status = "confirmed",
                    Round = round // Se utiliza la ronda actual
                });

                matchNumber++;
                // Incrementar el horario del partido en 30 minutos
                currentMatchTime = currentMatchTime.AddMinutes(30);

                // Si se supera el horario diario, avanzar al siguiente día y reiniciar al StartTime
                if (currentMatchTime.TimeOfDay >= tournamentEndTime)
                {
                    currentMatchTime = currentMatchTime.Date.AddDays(1) + tournamentStartTime;
                }
            }
        }

        public async Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Matches WHERE TournamentID = @TournamentId ORDER BY MatchNumber";
            return await connection.QueryAsync<MatchResponseDto>(query, new { TournamentId = tournamentId });
        }

        public async Task<MatchResponseDto?> GetMatchByIdAsync(int matchId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = "SELECT * FROM Matches WHERE ID = @MatchId";
            return await connection.QueryFirstOrDefaultAsync<MatchResponseDto>(query, new { MatchId = matchId });
        }

        public async Task<bool> UpdateMatchWinnerAsync(int matchId, int winnerId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" UPDATE Matches SET WinnerID = @WinnerId, Status = 'Finished' WHERE ID = @MatchId";
            int rowsAffected = await connection.ExecuteAsync(query, new { WinnerId = winnerId, MatchId = matchId });
            return rowsAffected > 0;
        }

        public async Task<List<int>> GetWinnersByRoundAsync(int tournamentId, int round)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();
            var query = @" SELECT WinnerID FROM Matches WHERE TournamentID = @TournamentId 
                           AND Round = @Round AND WinnerID IS NOT NULL";
            var winners = await connection.QueryAsync<int>(query, new { TournamentId = tournamentId, Round = round });
            return winners.ToList();
        }

        public async Task<bool> IsFirstRoundAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();

            // Consulta para contar cuántos partidos hay en el torneo
            var query = "SELECT COUNT(*) FROM Matches WHERE TournamentID = @TournamentId";

            // Ejecutamos la consulta y obtenemos el número de partidos
            int matchCount = await connection.ExecuteScalarAsync<int>(query, new { TournamentId = tournamentId });

            // Si no hay partidos, es la primera ronda
            return matchCount == 0;
        }

        public async Task<int> GetLastRoundAsync(int tournamentId)
        {
            using var connection = await _databaseConnection.GetConnectionAsync();

            // Consulta para obtener la última ronda (Round) agregada para el torneo
            var query = @"
        SELECT MIN(Round) 
        FROM Matches 
        WHERE TournamentID = @TournamentId
          AND Status = 'Finished'";

            // Ejecutar la consulta y obtener el resultado
            var lastRound = await connection.ExecuteScalarAsync<int?>(query, new { TournamentId = tournamentId });

            // Si no hay partidos registrados, devolver 0
            return lastRound ?? 0;
        }
    }
}
