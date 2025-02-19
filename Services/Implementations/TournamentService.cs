﻿
using Config;
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Tournament;
using Models.Enums;
using Models.Exceptions;
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
        public TournamentService(ITournamentDao tournamentDao, ICountryDao countryDao, ITournamentPlayerDao tournamentPlayerDao, IDeckDao deckDao)
        {
            _tournamentDao = tournamentDao;
            _countryDao = countryDao;
            _tournamentPlayerDao = tournamentPlayerDao;
            _deckDao = deckDao;
        }

        public async Task<TournamentResponseDto> CreateTournamentAsync(TournamentRequestDto dto, int organizerId)
        {
            // Validar que StartDate es anterior a EndDate
            if (dto.StartDate >= dto.EndDate)
            {
                throw new ValidationException("La fecha de inicio debe ser anterior a la fecha de fin");
            }
            // Validar que DailyEndTime es mayor que DailyStartTime
            if (dto.StartTime >= dto.EndTime)
            {
                throw new ValidationException("La hora de inicio diaria debe ser anterior a la hora de fin diaria");
            }

            if (!string.IsNullOrEmpty(dto.CountryCode))
            {
                bool countryExists = await _countryDao.CountryExists(dto.CountryCode);
                if (!countryExists)
                    throw new ValidationException(ErrorMessages.InvalidCountryCode);
            }

            int newTournament = await _tournamentDao.AddTournamentAsync(dto, organizerId);
            var tournament = await _tournamentDao.GetTournamentByIdAsync(newTournament);
            if (tournament == null)
            {
                throw new ValidationException("Error al obtener el torneo recién creado");
            }
            return tournament;
        }

        public async Task<TournamentResponseDto> GetTournamentByIdAsync(int tournamentId)
        {
            return await _tournamentDao.GetTournamentByIdAsync(tournamentId);
        }

        //public Task<bool>TournamentDateValidation(DateTime startDate, DateTime endDate)
        //{
        //    return (startDate >= endDate) ? throw new ValidationException("La fecha de inicio debe ser anterior a la fecha de fin");
        //}

        public async Task<bool> RegisterPlayerAsync(int tournamentId, int userId, int deckId)
        {
            var tournament = await _tournamentDao.GetTournamentByIdAsync(tournamentId);
            if (tournament == null)
                throw new NotFoundException("Torneo no encontrado");

            if (tournament.Phase != PhaseEnum.Registration)
                throw new ValidationException("Las inscripciones estan cerradas");

            if (await _tournamentPlayerDao.IsPlayerRegisteredAsync(tournamentId, userId))
                throw new ValidationException("El jugador ya esta inscripto en este torneo");

            if (!await _deckDao.IsDeckOwnedByUser(deckId, userId))
                throw new ValidationException("El mazo no esta permitido");

            if (tournament.CountPlayers >= tournament.MaxPlayers)
            {
                await _tournamentDao.UpdateTournamentPhaseAsync(tournamentId, PhaseEnum.Closed.ToString().ToLowerInvariant());
                throw new ValidationException("El torneo ha alcanzado su capacidad máxima");
            }

            await _tournamentDao.IncrementCountPlayersAsync(tournamentId);

            if (tournament.CountPlayers + 1 == tournament.MaxPlayers)
            {
                await _tournamentDao.UpdateTournamentPhaseAsync(tournamentId, PhaseEnum.Closed.ToString().ToLowerInvariant());
            }

            return await _tournamentPlayerDao.RegisterPlayerAsync(tournamentId, userId, deckId);

        }
    }
}
