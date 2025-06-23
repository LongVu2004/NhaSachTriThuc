using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NhaSachTriThuc.ViewModels
{
    public class BookViewModel
    {
        //public int? BookId { get; set; } // Nullable để dùng chung cho Create và Edit

        [Required(ErrorMessage = "Tiêu đề sách là bắt buộc.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Tác giả là bắt buộc.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Giá bán là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } // Đường dẫn ảnh đã lưu (dành cho Edit)

        public IFormFile? ImageFile { get; set; } // File ảnh upload mới

        public string? Description { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } // Danh sách danh mục đổ vào DropDownList
    }
}
