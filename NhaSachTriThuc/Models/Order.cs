namespace NhaSachTriThuc.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Thông tin người nhận hàng
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
