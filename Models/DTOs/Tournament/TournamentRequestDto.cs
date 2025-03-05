

namespace Models.DTOs.Tournament
{
    public class TournamentRequestDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CountryCode { get; set; }
    }
}
