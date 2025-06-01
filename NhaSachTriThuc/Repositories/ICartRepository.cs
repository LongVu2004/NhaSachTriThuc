using NhaSachTriThuc.Models;

namespace NhaSachTriThuc.Repositories
{
    public interface ICartRepository
    {
        List<CartItem> GetCart();
        void SaveCart(List<CartItem> cart);
        void AddToCart(Book book);
        void IncreaseQuantity(int bookId);
        void DecreaseQuantity(int bookId);
        void RemoveFromCart(int bookId);
        void ClearCart();
        decimal GetTotal();
    }
}
