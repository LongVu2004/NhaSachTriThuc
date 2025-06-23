using Microsoft.AspNetCore.Identity;

namespace NhaSachTriThuc.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;

        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public int Rating { get; set; } // Đánh giá từ 1 đến 5

    }
}
