
using Dapper;
using DataAccess.DAOs.Interfaces;
using DataAccess.Database;

namespace DataAccess.DAOs.Implementations
{
    public class MatchDao : IMatchDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public MatchDao(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds)
        {
            if (playerIds == null || playerIds.Count % 2 != 0)
                throw new Exception("La cantidad de jugadores debe ser par para formar partidos.");
            // Aca algo que se puede hacer es en caso de que solo hay un solo jugador dar el partido como ganado
            // Asi podes sumar mayor cantidad de partidos en un torneo
            // Continuando el formato de playoff pero recorriendo esto segun etapas
            // Es decir que si un match de 16avos de final no consigue un jugador
            // Este pasa directamente a octavos


            // Se asume que se tiene la información del torneo para asignar el horario de inicio
            using var connection = await _databaseConnection.GetConnectionAsync();

            // Consultamos los datos del torneo (para obtener el horario de inicio y maxPlayers)
            var tournamentQuery = "SELECT StartDate, StartTime, MaxPlayers FROM Tournament WHERE ID = @TournamentId";
            var tournament = await connection.QueryFirstOrDefaultAsync<dynamic>(tournamentQuery, new { TournamentId = tournamentId });
            if (tournament == null)
                throw new Exception("Torneo no encontrado");

            // Programar el MatchStartTime como la combinación de StartDate y StartTime
            DateTime matchStartTime = ((DateTime)tournament.StartDate).Date +
                                          ((TimeSpan)tournament.StartTime);

            // Se calcula el total de partidos del bracket: TotalMatches = MaxPlayers - 1
            int totalMatches = (int)tournament.MaxPlayers - 1;

            // Primera ronda: cada partido reúne 2 jugadores, cantidad de partidos es playerIds.Count /
            int matchNumber = 1;
            var insertQuery = @"INSERT INTO Matches (TournamentID, MatchNumber, TotalMatches, Player1ID, Player2ID, ScheduledStartTime, Status)
                                VALUES (@TournamentID, @MatchNumber, @TotalMatches, @Player1ID, @Player2ID, @ScheduledStartTime, @Status)";

            // Recorrer la lista de jugadores emparejándolos
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
                    ScheduledStartTime = matchStartTime,  // O puedes calcular distintos horarios si se juegan en serie
                    Status = "confirmed"  // Usamos string; alternativamente, MatchStatusEnum.Confirmed.ToString().ToLowerInvariant()
                });

                matchNumber++;
            }
        }
    }
}
