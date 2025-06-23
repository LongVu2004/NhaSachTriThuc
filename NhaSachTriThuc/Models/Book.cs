using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaSachTriThuc.Models
{
    public class Book
    {
        public int BookId { get; set; }
        [Required(ErrorMessage = "Tiêu đề sách là bắt buộc.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Tác giả là bắt buộc.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Giá bán là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public List<OrderDetail>? OrderDetails { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
