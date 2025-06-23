namespace NhaSachTriThuc.Models
{
    public class RevenueReport
    {
        public List<string> Labels { get; set; } // Tháng hoặc ngày
        public List<decimal> Revenues { get; set; } // Doanh thu tương ứng
    }
}
