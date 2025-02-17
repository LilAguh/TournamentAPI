

namespace Models.DTOs.Tournament
{
    public class TournamentRequestDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string CountryCode { get; set; }
    }
}
