using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaSachTriThuc.Data;

namespace NhaSachTriThuc.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult History()
        {
            string userId = _userManager.GetUserId(User); // Lấy Id từ bảng AspNetUsers
            var orders = _context.Orders
                                 .Where(o => o.UserId == userId)
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToList();
            return View(orders);
        }
        [Authorize]
        public IActionResult Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Book)
                .FirstOrDefault(o => o.OrderId == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

    }
}
