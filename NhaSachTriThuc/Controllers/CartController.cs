using Microsoft.AspNetCore.Mvc;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;
using NhaSachTriThuc.Repositories;

namespace NhaSachTriThuc.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICartRepository _cartRepo;

        public CartController(AppDbContext context, ICartRepository cartRepo)
        {
            _context = context;
            _cartRepo = cartRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = _cartRepo.GetCart();
            ViewBag.Total = _cartRepo.GetTotal();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == id);
            if (book == null)
            {
                TempData["Error"] = "Sách không tồn tại.";
                return RedirectToAction("Details", "Product", new { id }); // Quay lại trang chi tiết
            }

            _cartRepo.AddToCart(book);
            TempData["Message"] = "Đã thêm sách vào giỏ hàng!";
            return RedirectToAction("Details", "Product", new { id }); // Quay lại trang chi tiết
        }

        [HttpPost]
        public IActionResult AddToCartAjax(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == id);
            if (book == null)
                return Json(new { success = false, message = "Sách không tồn tại" });

            _cartRepo.AddToCart(book);
            return Json(new { success = true, message = "Đã thêm sách vào giỏ!" });
        }

        // Hiển thị form thanh toán
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = _cartRepo.GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index");
            }

            ViewBag.Total = _cartRepo.GetTotal();
            return View(); // Sẽ tạo View bên dưới
        }

        // Xử lý form thanh toán
        [HttpPost]
        public IActionResult Checkout(string customerName, string customerPhone, string customerAddress)
        {
            var cart = _cartRepo.GetCart();
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerName = customerName,
                CustomerPhone = customerPhone,
                CustomerAddress = customerAddress,
                Status = OrderStatus.Pending,
                TotalAmount = _cartRepo.GetTotal(),
                OrderDetails = cart.Select(item => new OrderDetail
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            _cartRepo.ClearCart(); // Xóa giỏ hàng

            return RedirectToAction("Success", new { id = order.OrderId });
        }

        // Hiển thị xác nhận đơn hàng
        public IActionResult Success(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            return View(order); // Tạo view Success.cshtml
        }


        [HttpPost]
        public IActionResult Remove(int id)
        {
            _cartRepo.RemoveFromCart(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            _cartRepo.ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult IncreaseQuantity(int id)
        {
            _cartRepo.IncreaseQuantity(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DecreaseQuantity(int id)
        {
            _cartRepo.DecreaseQuantity(id);
            return RedirectToAction("Index");
        }
    }
}
