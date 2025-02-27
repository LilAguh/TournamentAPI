
namespace Models.DTOs.Disqualifications
{
    public class DisqualificationRequestDto
    {
        public int TournamentID { get; set; }
        public int PlayerID { get; set; }
        public string Reason { get; set; }
    }
}
