
using Config;
using DataAccess.DAOs.Interfaces;
using Models.DTOs.CardDecks;
using Models.DTOs.Tournament;
using Models.Enums;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentDao _tournamentDao;
        private readonly ICountryDao _countryDao;
        private readonly ITournamentPlayerDao _tournamentPlayerDao;
        private readonly IDeckDao _deckDao;
        private readonly ICardDeckDao _cardDeckDao;
        private readonly ICardSeriesDao _cardSeriesDao;

        public TournamentService(ITournamentDao tournamentDao, ICountryDao countryDao, ITournamentPlayerDao tournamentPlayerDao, IDeckDao deckDao, ICardDeckDao cardDeckDao, ICardSeriesDao cardSeriesDao)
        {
            _tournamentDao = tournamentDao;
            _countryDao = countryDao;
            _tournamentPlayerDao = tournamentPlayerDao;
            _deckDao = deckDao;
            _cardDeckDao = cardDeckDao;
            _cardSeriesDao = cardSeriesDao;
        }

        // Crea un torneo, validando que la creación sea válida y calculando la capacidad del torneo.
        public async Task<TournamentResponseDto> CreateTournamentAsync(TournamentRequestDto dto, int organizerId)
        {
            await ValidateTournamentCreation(dto);

            var (maxPlayers, maxGames) = CalculateTournamentCapacity(dto);
            var tournamentId = await _tournamentDao.AddTournamentAsync(dto, organizerId, maxPlayers, maxGames);

            return await GetAndValidateTournament(tournamentId);
        }

        // Obtiene un torneo por ID, validando si existe.
        public async Task<TournamentResponseDto> GetTournamentByIdAsync(int tournamentId)
        {
            return await GetAndValidateTournament(tournamentId);
        }

        // Registra un jugador en el torneo, validando que cumpla con las reglas y los requisitos del torneo.
        public async Task<bool> RegisterPlayerAsync(int tournamentId, int userId, int deckId)
        {
            var tournament = await GetAndValidateTournament(tournamentId);

            await ValidateRegistration(tournament, userId, deckId);
            await ProcessPlayerRegistration(tournament, userId, deckId);

            return true;
        }

        // Agrega una serie permitida al torneo.
        public async Task AddAllowedSeriesAsync(int tournamentId, int seriesId)
        {
            await _tournamentDao.AddAllowedSeriesAsync(tournamentId, seriesId);
        }

        // Elimina una serie permitida del torneo.
        public async Task RemoveAllowedSeriesAsync(int tournamentId, int seriesId)
        {
            await _tournamentDao.RemoveAllowedSeriesAsync(tournamentId, seriesId);
        }

        // Obtiene las series permitidas de un torneo.
        public async Task<List<int>> GetAllowedSeriesAsync(int tournamentId)
        {
            return await _tournamentDao.GetAllowedSeriesAsync(tournamentId);
        }

        // Métodos privados //

        // Valida la creación del torneo, asegurando fechas y códigos de país válidos.
        private async Task ValidateTournamentCreation(TournamentRequestDto dto)
        {
            ValidateTournamentDates(dto);
            await ValidateCountryCode(dto.CountryCode);
        }

        // Valida las fechas de inicio y fin del torneo.
        private void ValidateTournamentDates(TournamentRequestDto dto)
        {
            if (dto.StartDate >= dto.EndDate)
                throw new ValidationException("La fecha de inicio debe ser anterior a la fecha de fin");
        }

        // Valida si el código de país es válido.
        private async Task ValidateCountryCode(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode) && !await _countryDao.CountryExists(countryCode))
                throw new ValidationException(ErrorMessages.InvalidCountryCode);
        }

        // Calcula la capacidad del torneo en términos de jugadores y juegos.
        private (int maxPlayers, int maxGames) CalculateTournamentCapacity(TournamentRequestDto dto)
        {
            int dayAvailable = 1 + (dto.EndDate - dto.StartDate).Days;
            int minutesPerDay = (dto.EndDate.Hour * 60 + dto.EndDate.Minute) -
                               (dto.StartDate.Hour * 60 + dto.StartDate.Minute);

            int totalMatches = (dayAvailable * minutesPerDay) / 30;
            int maxPlayers = CalculateMaxPlayers(totalMatches);

            return (maxPlayers, maxPlayers - 1);
        }

        // Calcula el máximo número de jugadores en función de la cantidad total de partidos.
        private int CalculateMaxPlayers(int totalMatches)
        {
            int maxPlayers = 2;
            while ((maxPlayers - 1) <= totalMatches)
            {
                maxPlayers *= 2;
            }
            return maxPlayers / 2;
        }

        // Obtiene y valida la existencia de un torneo por ID.
        private async Task<TournamentResponseDto> GetAndValidateTournament(int tournamentId)
        {
            var tournament = await _tournamentDao.GetTournamentByIdAsync(tournamentId);
            return tournament ?? throw new NotFoundException("Torneo no encontrado");
        }

        // Valida que el registro de un jugador en el torneo sea válido.
        private async Task ValidateRegistration(TournamentResponseDto tournament, int userId, int deckId)
        {
            ValidateRegistrationPhase(tournament);
            await ValidatePlayerEligibility(tournament.Id, userId);
            await ValidateDeckCompliance(deckId, userId, tournament.Id);
        }

        // Valida que el torneo esté en la fase de inscripción.
        private void ValidateRegistrationPhase(TournamentResponseDto tournament)
        {
            if (tournament.Phase != PhaseEnum.Registration)
                throw new ValidationException("Las inscripciones están cerradas");
        }

        // Valida si el jugador ya está registrado en el torneo.
        private async Task ValidatePlayerEligibility(int tournamentId, int userId)
        {
            if (await _tournamentPlayerDao.IsPlayerRegisteredAsync(tournamentId, userId))
                throw new ValidationException("El jugador ya está registrado en este torneo");
        }

        // Valida que el mazo del jugador sea válido y cumpla con los requisitos del torneo.
        private async Task ValidateDeckCompliance(int deckId, int userId, int tournamentId)
        {
            await ValidateDeckOwnership(deckId, userId);
            await ValidateDeckContent(deckId, tournamentId);
        }

        // Valida que el jugador sea dueño del mazo.
        private async Task ValidateDeckOwnership(int deckId, int userId)
        {
            if (!await _deckDao.IsDeckOwnedByUser(deckId, userId))
                throw new ValidationException("El mazo no está permitido");
        }

        // Valida que el mazo cumpla con las reglas del torneo en términos de contenido.
        private async Task ValidateDeckContent(int deckId, int tournamentId)
        {
            var deckCards = await _cardDeckDao.GetCardsInDeckAsync(deckId);
            ValidateDeckSize(deckCards);
            await ValidateCardSeriesCompliance(deckCards, tournamentId);
        }

        // Valida que el mazo tenga el tamaño correcto.
        private void ValidateDeckSize(IEnumerable<CardDeckResponseDto> deckCards)
        {
            if (deckCards.Count() != 15)
                throw new ValidationException("El mazo debe tener exactamente 15 cartas");
        }

        // Valida que las cartas del mazo pertenezcan a las series permitidas en el torneo.
        private async Task ValidateCardSeriesCompliance(IEnumerable<CardDeckResponseDto> deckCards, int tournamentId)
        {
            var allowedSeries = await GetAllowedSeriesAsync(tournamentId);

            foreach (var card in deckCards)
            {
                var cardSeries = await _cardSeriesDao.GetSeriesByCardAsync(card.CardId);
                if (!cardSeries.Any(s => allowedSeries.Contains(s.SeriesId)))
                    throw new ValidationException($"La carta {card.CardName} no pertenece a las series permitidas");
            }
        }

        // Procesa el registro de un jugador, asegurando que la capacidad del torneo no se haya alcanzado.
        private async Task ProcessPlayerRegistration(TournamentResponseDto tournament, int userId, int deckId)
        {
            await HandleRegistrationCapacity(tournament);
            await _tournamentPlayerDao.RegisterPlayerAsync(tournament.Id, userId, deckId);
        }

        // Maneja la capacidad de registro del torneo, cerrando inscripciones si es necesario.
        private async Task HandleRegistrationCapacity(TournamentResponseDto tournament)
        {
            if (tournament.CountPlayers >= tournament.MaxPlayers)
                await CloseTournamentRegistration(tournament.Id);

            await _tournamentDao.IncrementCountPlayersAsync(tournament.Id);

            if (tournament.CountPlayers + 1 == tournament.MaxPlayers)
                await CloseTournamentRegistration(tournament.Id);
        }

        // Cierra las inscripciones del torneo cuando se alcanza la capacidad máxima.
        private async Task CloseTournamentRegistration(int tournamentId)
        {
            await _tournamentDao.UpdateTournamentPhaseAsync(
                tournamentId,
                PhaseEnum.Closed.ToString().ToLowerInvariant()
            );
            throw new ValidationException("El torneo ha alcanzado su capacidad máxima");
        }
    }
}
