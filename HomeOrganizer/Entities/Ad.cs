namespace HomeOrganizer.Entities
{
    public class Ad
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string TextBody { get; set; }
        public bool IsVoting { get; set; }
        public Voting? Voting { get; set; }
        public List<string>? Acquainted { get; set; }

        public int GroupId { get; set; }
        public Group? Group { get; set; }

    }
}
