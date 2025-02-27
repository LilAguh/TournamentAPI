
namespace Models.Entities
{
    public class Disqualification
    {
        public int Id { get; set; }
        public int TournamentID { get; set; }
        public int PlayerID { get; set; }
        public int JudgeID { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}