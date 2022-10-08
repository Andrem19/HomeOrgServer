namespace HomeOrganizer.Entities
{
    public class Voting
    {
        public int Id { get; set; }
        public List<Variant> Variants { get; set; } = new();
        public bool IsSecret { get; set; }
        public List<string> IsVote { get; set; } = new();

        public int AdId { get; set; }
        public Ad? Ad { get; set; }
    }
}