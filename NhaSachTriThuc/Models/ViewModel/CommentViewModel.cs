using System.ComponentModel.DataAnnotations;

namespace NhaSachTriThuc.Models.ViewModel
{
    public class CommentViewModel
    {
        public int BookId { get; set; }
        public string Content { get; set; }

        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")]
        public int Rating { get; set; }
    }
}
