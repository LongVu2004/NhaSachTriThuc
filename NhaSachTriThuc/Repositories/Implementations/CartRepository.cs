using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;

namespace NhaSachTriThuc.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public CartRepository(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public List<CartItem> GetCart()
        {
            var cartJson = Session.GetString("cart");
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        public void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            Session.SetString("cart", cartJson);
        }

        public void AddToCart(Book book)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.BookId == book.BookId);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Price = book.Price,
                    Quantity = 1,
                    Book = book
                });
            }
            SaveCart(cart);
        }

        public void RemoveFromCart(int bookId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.BookId == bookId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            SaveCart(new List<CartItem>());
        }

        public decimal GetTotal()
        {
            var cart = GetCart();
            return cart.Sum(item => item.Price * item.Quantity);
        }

        public void IncreaseQuantity(int bookId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.BookId == bookId);
            if (item != null)
            {
                item.Quantity++;
                SaveCart(cart);
            }
        }

        public void DecreaseQuantity(int bookId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.BookId == bookId);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
                SaveCart(cart);
            }
        }
    }
}
