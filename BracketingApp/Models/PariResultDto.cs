namespace BracketingApp.Models
{
    public class PairResultDto
    {
        public Pair<string, string> Pair { get; set; }
        public string Winner { get; set; }
        public string Loser { get; set; }
    }
}