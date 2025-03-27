

using DataAccess.DAOs.Interfaces;
using Models.DTOs.Matches;
using Models.DTOs.Tournament;
using Models.Enums;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class MatchService : IMatchService
    {
        private readonly IMatchDao _matchDao;
        private readonly ITournamentPlayerDao _tournamentPlayerDao;
        private readonly ITournamentDao _tournamentDao;

        public MatchService(IMatchDao matchDao, ITournamentPlayerDao tournamentPlayerDao, ITournamentDao tournamentDao)
        {
            _matchDao = matchDao;
            _tournamentPlayerDao = tournamentPlayerDao;
            _tournamentDao = tournamentDao;
        }

        public async Task CreateRoundMatchAsync(int tournamentId)
        {
            var tournament = await GetValidTournament(tournamentId);
            var playerIds = await GetValidPlayers(tournamentId);
            var round = CalculateRound(tournament, playerIds);
            var timeSlots = GenerateTimeSlots(tournament);

            var matchesToCreate = BuildMatchDtos(tournament, playerIds, round, timeSlots);
            await _matchDao.CreateMatchesAsync(matchesToCreate);
        }

        public async Task<IEnumerable<MatchResponseDto>> GetMatchesByTournamentAsync(int tournamentId)
        {
            return await _matchDao.GetMatchesByTournamentAsync(tournamentId);
        }

        public async Task<bool> UpdateMatchWinnerAsync(MatchResultRequestDto dto)
        {
            var existingMatch = await ValidateMatch(dto.MatchId);
            ValidateWinner(existingMatch, dto.WinnerId);

            var updated = await _matchDao.UpdateMatchResultAsync(dto);

            if (IsFinalRound(existingMatch))
                await FinalizeTournament(existingMatch.TournamentID, dto.WinnerId);

            return updated;
        }


        private async Task<TournamentResponseDto> GetValidTournament(int tournamentId)
        {
            var tournament = await _tournamentDao.GetTournamentByIdAsync(tournamentId);
            return tournament ?? throw new NotFoundException("Torneo no encontrado");
        }

        private async Task<List<int>> GetValidPlayers(int tournamentId)
        {
            var playerIds = (await _tournamentPlayerDao.GetPlayerIdsAsync(tournamentId))
                .Distinct()
                .ToList();

            if (playerIds.Count % 2 != 0)
                throw new ValidationException("Número impar de jugadores para crear partidos");

            return playerIds;
        }

        private int CalculateRound(TournamentResponseDto tournament, List<int> playerIds)
        {
            bool isFirstRound = playerIds.Count == tournament.MaxPlayers;
            return isFirstRound ? tournament.MaxPlayers / 2 : 1;
        }

        private List<DateTime> GenerateTimeSlots(TournamentResponseDto tournament)
        {
            var slots = new List<DateTime>();
            var currentDay = tournament.StartDate.Date;
            var endDay = tournament.EndDate.Date.AddDays(1);

            while (currentDay < endDay)
            {
                var startTime = currentDay.Add(tournament.StartDate.TimeOfDay);
                var endTime = currentDay.Add(tournament.EndDate.TimeOfDay);

                while (startTime <= endTime)
                {
                    slots.Add(startTime);
                    startTime = startTime.AddMinutes(30);
                }
                currentDay = currentDay.AddDays(1);
            }

            return slots;
        }

        private List<MatchCreateRequestDto> BuildMatchDtos(
            TournamentResponseDto tournament,
            List<int> playerIds,
            int round,
            List<DateTime> timeSlots)
        {
            var matches = new List<MatchCreateRequestDto>();
            var rnd = new Random();

            if (round == tournament.MaxPlayers / 2)
                playerIds = playerIds.OrderBy(_ => rnd.Next()).ToList();

            for (int i = 0; i < playerIds.Count; i += 2)
            {
                if (!timeSlots.Any())
                    throw new ValidationException("No hay suficientes horarios disponibles");

                matches.Add(new MatchCreateRequestDto
                {
                    TournamentId = tournament.Id,
                    MatchNumber = matches.Count + 1,
                    TotalMatches = tournament.MaxPlayers - 1,
                    Player1Id = playerIds[i],
                    Player2Id = playerIds[i + 1],
                    MatchStartTime = timeSlots[0],
                    Status = MatchEnum.Confirmed,
                    Round = round
                });

                timeSlots.RemoveAt(0);
            }

            return matches;
        }

        private async Task<MatchResponseDto> ValidateMatch(int matchId)
        {
            var match = await _matchDao.GetMatchByIdAsync(matchId);
            return match ?? throw new NotFoundException("Partido no encontrado");
        }

        private void ValidateWinner(MatchResponseDto match, int winnerId)
        {
            if (match.Player1ID != winnerId && match.Player2ID != winnerId)
                throw new ValidationException("El jugador no participa en este partido");
        }

        private bool IsFinalRound(MatchResponseDto match)
        {
            return match.Round == 1;
        }

        private async Task FinalizeTournament(int tournamentId, int winnerId)
        {
            await _tournamentDao.FinalizeTournamentAsync(tournamentId, winnerId);
        }

    }
}
