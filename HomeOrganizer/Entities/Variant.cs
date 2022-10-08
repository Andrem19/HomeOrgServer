namespace HomeOrganizer.Entities
{
    public class Variant
    {
        public int Id { get; set; }
        public string TextBody { get; set; }
        public double Percent { get; set; }
        public List<string>? Names { get; set; }

        public int VotingId { get; set; }
        public Voting? Voting { get; set; }
    }
}