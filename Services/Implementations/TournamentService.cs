
using Config;
using DataAccess.DAOs.Interfaces;
using Models.DTOs.Tournament;
using Services.Interfaces;
using static Models.Exceptions.CustomException;

namespace Services.Implementations
{
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentDao _tournamentDao;
        private readonly ICountryDao _countryDao;
        public TournamentService(ITournamentDao tournamentDao, ICountryDao countryDao)
        {
            _tournamentDao = tournamentDao;
            _countryDao = countryDao;
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
    }
}
