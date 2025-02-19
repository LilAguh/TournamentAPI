
using Models.Enums;

namespace Models.Entities
{
    public class Tournament
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
        public PhaseEnum Phase { get; set; }
        public int WinnerID { get; set; }
    }
}
