
using Models.Enums;

namespace Models.DTOs.Matches
{
    public class MatchCreateRequestDto
    {
        public int TournamentId { get; set; }
        public int MatchNumber { get; set; }
        public int TotalMatches { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public DateTime MatchStartTime { get; set; }
        public MatchEnum Status { get; set; }
        public int Round { get; set; }
    }
}
