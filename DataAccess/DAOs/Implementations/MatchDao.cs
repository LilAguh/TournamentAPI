
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

        public async Task CreateRoundMatchAsync(int tournamentId, List<int> playerIds)
        {
            if (playerIds == null)
                throw new Exception("La lista de jugadores no puede ser nula");

            // Aca algo que se puede hacer es en caso de que solo hay un solo jugador dar el partido como ganado
            // Asi podes sumar mayor cantidad de partidos en un torneo
            // Continuando el formato de playoff pero recorriendo esto segun etapas
            // Es decir que si un match de 16avos de final no consigue un jugador
            // Este pasa directamente a octavos


            // Eliminar duplicados para evitar que un jugador se repita
            playerIds = playerIds.Distinct().ToList();


            // Validar que la cantidad de jugadores es par
            if (playerIds.Count % 2 != 0)
                throw new Exception("La cantidad de jugadores debe ser par para formar los partidos");

            // Mezclar aleatoriamente los IDs de los jugadores (para la primera ronda)
            var randomizedPlayerIds = playerIds.OrderBy(x => Guid.NewGuid()).ToList();

            // Se asume que se tiene la información del torneo para asignar el horario de inicio
            using var connection = await _databaseConnection.GetConnectionAsync();

            // Consultamos los datos del torneo (para obtener el horario de inicio y maxPlayers)
            var tournamentQuery = "SELECT StartDate, StartTime, EndTime, MaxPlayers FROM Tournament WHERE ID = @TournamentId";
            var tournament = await connection.QueryFirstOrDefaultAsync<dynamic>(tournamentQuery, new { TournamentId = tournamentId });
            if (tournament == null)
                throw new NotFoundException("Torneo no encontrado");


            // Verificar que StartTime y EndTime no sean nulos
            if (tournament.StartTime == null)
                throw new NotFoundException("StartTime no definido en el torneo");
            if (tournament == null)
                throw new NotFoundException("EndTime no definido en el torneo");

            // Programar el MatchStartTime como la combinación de StartDate y StartTime
            DateTime tournamentStartDate = tournament.StartDate;
            TimeSpan tournamentStartTime = (TimeSpan)tournament.StartTime;
            TimeSpan tournamentEndTime = (TimeSpan)tournament.EndTime;

            DateTime currentMatchTime = tournamentStartDate.Date + tournamentStartTime;

            // Se calcula el total de partidos del bracket: TotalMatches = MaxPlayers - 1
            int totalMatches = (int)(tournament.MaxPlayers)  - 1;

            // Primera ronda: cada partido reúne 2 jugadores, cantidad de partidos es playerIds.Count /
            int matchNumber = 1;
            var insertQuery = @"INSERT INTO Matches (TournamentID, MatchNumber, TotalMatches, Player1ID, Player2ID, MatchStartTime, Status)
                                VALUES (@TournamentID, @MatchNumber, @TotalMatches, @Player1ID, @Player2ID, @MatchStartTime, @Status)";

            // Recorrer la lista de jugadores emparejándolos
            for (int i = 0; i < randomizedPlayerIds.Count; i +=2)
            {
                int player1Id = randomizedPlayerIds[i];
                int player2Id = randomizedPlayerIds[i + 1];

                await connection.ExecuteAsync(insertQuery, new
                {
                    TournamentID = tournamentId,
                    MatchNumber = matchNumber,
                    TotalMatches = totalMatches,
                    Player1ID = player1Id,
                    Player2ID = player2Id,
                    MatchStartTime = currentMatchTime,  // O puedes calcular distintos horarios si se juegan en serie
                    Status = "confirmed"  // Usamos string; alternativamente, MatchStatusEnum.Confirmed.ToString().ToLowerInvariant()
                });

                matchNumber++;
                // Incrementar la hora del partido en 30 minutos
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
    }
}
