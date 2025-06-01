using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;

namespace NhaSachTriThuc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Kiểm tra quyền admin
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            //return role == "Admin";
            return role == "Admin";
        }

        public IActionResult Index()
        {
            var totalBooks = _context.Books.Count();
            var totalOrders = _context.Orders.Count();
            var pendingOrders = _context.Orders.Count(o => o.Status == OrderStatus.Pending);

            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.PendingOrders = pendingOrders;

            return View();
        }

        // Danh sách sản phẩm
        public IActionResult Products()
        {
            var books = _context.Books.Include(b => b.Category).ToList();
            return View(books);
        }

        // GET: Thêm sách mới
        [HttpGet]
        public IActionResult AddBook()
        {
            ViewBag.Categories = _context.Categories.ToList(); // Truyền danh sách Category
            return View();
        }

        // POST: Thêm sách mới
        [HttpPost]
        public IActionResult AddBook(Book book, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Tạo đường dẫn lưu file
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                // Lưu file vào wwwroot/images
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                // Gán đường dẫn tương đối vào Book.ImageUrl
                book.ImageUrl = "/images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction("Products");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // GET: Sửa sách
        [HttpGet]
        public IActionResult EditBook(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == id);
            if (book == null)
                return NotFound();

            ViewBag.Categories = _context.Categories.ToList(); // Truyền danh sách Category
            return View(book);
        }

        // POST: Sửa sách
        [HttpPost]
        public IActionResult EditBook(Book book, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                book.ImageUrl = fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction("Products");
            }

            ViewBag.Categories = _context.Categories.ToList(); // Bổ sung dòng này khi return View(book)
            return View(book);
        }

        // GET: Xóa sách
        [HttpGet]
        public IActionResult DeleteBook(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: Xác nhận xóa sách
        [HttpPost]
        public IActionResult DeleteBookConfirmed(int BookId)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == BookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }

            return RedirectToAction("Products");
        }

        public IActionResult Dashboard()
        {
            var totalBooks = _context.Books.Count();
            var totalOrders = _context.Orders.Count();
            var pendingOrders = _context.Orders.Count(o => o.Status == OrderStatus.Pending);

            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.PendingOrders = pendingOrders;

            return View();
        }

        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .ToList();

            return View(orders);
        }

        [HttpGet]
        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null) return NotFound();

            ViewBag.StatusList = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();
            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null) return NotFound();

            order.Status = status;
            _context.SaveChanges();

            return RedirectToAction("Orders");
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var model = new List<(IdentityUser user, IList<string> roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add((user, roles));
            }

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        // POST: /Admin/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(string email, string password, string role)
        {
            if (!ModelState.IsValid) return RedirectToAction("Users");

            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);
            }

            return RedirectToAction("Users");
        }

        // POST: /Admin/ChangeRole
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            return RedirectToAction("Users");
        }
    }
}
