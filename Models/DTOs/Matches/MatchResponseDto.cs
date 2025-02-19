
using Models.Enums;

namespace Models.DTOs.Matches
{
    public class MatchResponseDto
    {
        public int Id { get; set; }
        public int TournamentID { get; set; }
        public int MatchNumber { get; set; }
        public int TotalMatches { get; set; }
        public int Player1ID { get; set; }
        public int Player2ID { get; set; }
        public DateTime MatchStartTime { get; set; }
        public MatchEnum Status { get; set; }
        public int WinnerID { get; set; }
    }
}
