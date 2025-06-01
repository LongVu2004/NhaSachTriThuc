namespace NhaSachTriThuc.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

    }
}
