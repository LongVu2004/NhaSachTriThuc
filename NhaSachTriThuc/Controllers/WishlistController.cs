using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Helpers;
using NhaSachTriThuc.Models;
using System.Security.Claims;

namespace NhaSachTriThuc.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WishlistController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: /Wishlist/
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var wishlist = await _context.Wishlists
                .Include(w => w.Book)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(wishlist);
        }

        // POST: /Wishlist/Add/5
        [HttpPost]
        public async Task<IActionResult> Add(int bookId)
        {
            var userId = _userManager.GetUserId(User);

            var existing = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.BookId == bookId && w.UserId == userId);

            if (existing == null)
            {
                var wishlistItem = new Wishlist
                {
                    BookId = bookId,
                    UserId = userId
                };

                _context.Wishlists.Add(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        // POST: /Wishlist/Remove/5
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var wishlistItem = await _context.Wishlists.FindAsync(id);
            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSelectedToCart(int[] selectedBookIds)
        {   

            if (selectedBookIds == null || selectedBookIds.Length == 0)
            {
                TempData["Message"] = "Vui lòng chọn ít nhất một sách.";
                return RedirectToAction("Index");
            }

            // Lấy giỏ hàng từ Session hoặc tạo mới nếu chưa có
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            foreach (var bookId in selectedBookIds)
            {
                var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book == null) continue;

                var existingItem = cart.FirstOrDefault(ci => ci.BookId == bookId);
                if (existingItem != null)
                {
                    existingItem.Quantity++; // tăng số lượng nếu đã có
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

                // Xóa khỏi wishlist
                var wishlistItem = _context.Wishlists
                    .FirstOrDefault(w => w.BookId == bookId && w.UserId == _userManager.GetUserId(User));
                if (wishlistItem != null)
                {
                    _context.Wishlists.Remove(wishlistItem);
                }
            }

            _context.SaveChanges();

            // Cập nhật lại giỏ hàng trong Session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }
    }
}
