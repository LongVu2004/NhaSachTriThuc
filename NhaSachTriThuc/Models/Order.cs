using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NhaSachTriThuc.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaSachTriThuc.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        //[Required]
        //public string UserId { get; set; } // foreign key

        //[ForeignKey("UserId")]
        //[ValidateNever] // để tránh lỗi trong Razor view khi binding
        //public ApplicationUser ApplicationUser { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        

        public List<OrderDetail> OrderDetails { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
    public enum OrderStatus
    {
        Pending,        // Đang chờ xử lý
        Confirmed,      // Đã xác nhận
        Shipping,       // Đang giao hàng
        Delivered,      // Đã giao hàng
        Cancelled       // Đã hủy
    }
}
