
using Models.Enums;

namespace Models.DTOs.Tournament
{
    public class TournamentResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizerID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string CountryCode { get; set; }
        public int WinnerID { get; set; }
    }
}
