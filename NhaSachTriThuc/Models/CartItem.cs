using Microsoft.AspNetCore.Identity;

namespace NhaSachTriThuc.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Book Book { get; set; }

    }
}
