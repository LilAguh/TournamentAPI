
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
        public int MaxPlayers { get; set; }
        public int MaxGames { get; set; }
        public int CountPlayers { get; set; }
        public int WinnerID { get; set; }
        public PhaseEnum Phase { get; set; }
        public List<int> AllowedSeriesIds { get; set; } = new List<int>();
        public List<int> Juedges { get; set; } = new List<int>();
    }
}
