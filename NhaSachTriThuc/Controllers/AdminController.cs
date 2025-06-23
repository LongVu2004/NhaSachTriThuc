using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;
using NhaSachTriThuc.Repositories;
using NhaSachTriThuc.ViewModels;

namespace NhaSachTriThuc.Controllers
{
    
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IBookRepository _bookRepository;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager,
                                RoleManager<IdentityRole> roleManager, IBookRepository bookRepository,
                                ICategoryRepository categoryRepo, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _bookRepository = bookRepository;
            _categoryRepo = categoryRepo;
            _env = env;
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

        [HttpGet]
        public IActionResult GetRevenueData(string type)
        {
            var orders = _context.Orders
                .Where(o => o.OrderDate != null && o.Status != OrderStatus.Cancelled)
                .ToList();

            var grouped = type == "day"
                ? orders.GroupBy(o => o.OrderDate.Date)
                : orders.GroupBy(o => new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1));

            var data = grouped
                .OrderBy(g => g.Key)
                .Select(g => new {
                    Label = type == "day" ? g.Key.ToString("dd/MM/yyyy") : g.Key.ToString("MM/yyyy"),
                    Total = g.Sum(o => o.TotalAmount)
                }).ToList();

            return Json(new
            {
                labels = data.Select(x => x.Label),
                values = data.Select(x => x.Total)
            });
        }

        // Danh sách sản phẩm
        public async Task<IActionResult> Products()
        {
            var books = await _bookRepository.GetAllBooks();
            return View(books);
        }

        // GET: Thêm sách mới
        //[HttpGet]
        //public IActionResult AddBook()
        //{
        //    ViewBag.Categories = _context.Categories.ToList();
        //    return View();
        //}// GET: Books/Create
        // GET: Books/AddBook
        public async Task<IActionResult> AddBook()
        {
            var categories = await _categoryRepo.GetAllAsync();
            var model = new BookViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList()
            };
            return View(model);
        }

        //// POST: Books/AddBook
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddBook(BookViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        //        //{
        //        //    Console.WriteLine(error.ErrorMessage); // Xem lỗi gì
        //        //}
        //        foreach (var value in ModelState.Values)
        //        {
        //            foreach (var error in value.Errors)
        //            {
        //                Console.WriteLine(error.ErrorMessage); // hoặc log ra file
        //            }
        //        }
        //        var categories = await _categoryRepo.GetAllAsync();
        //        model.Categories = categories.Select(c => new SelectListItem
        //        {
        //            Value = c.CategoryId.ToString(),
        //            Text = c.Name
        //        }).ToList();
        //        return View(model);
        //    }

        //    string imagePath = null!;
        //    if (model.ImageFile != null)
        //    {
        //        string uploadDir = Path.Combine(_env.WebRootPath, "images/books");
        //        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

        //        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
        //        string filePath = Path.Combine(uploadDir, uniqueFileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await model.ImageFile.CopyToAsync(stream);
        //        }

        //        imagePath = $"/images/books/{uniqueFileName}";
        //    }

        //    var book = new Book
        //    {
        //        //BookId = model.BookId ?? 0, // Nếu BookId là null thì gán 0
        //        Title = model.Title,
        //        Author = model.Author,
        //        Price = model.Price,
        //        Quantity = model.Quantity,
        //        Description = model.Description,
        //        CategoryId = model.CategoryId,
        //        ImageUrl = imagePath
        //    };

        //    Console.WriteLine("Đang thêm sách: " + model.Title);
        //    try
        //    {
        //        await _bookRepository.AddBookAsync(book);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Lỗi khi lưu DB: " + ex.Message);
        //        // Hoặc log chi tiết hơn
        //    }
        //    //await _context.SaveChangesAsync();
        //    return RedirectToAction("Index","Admin"); // hoặc action hiển thị danh sách
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Load lại danh sách Category khi ModelState không hợp lệ
                var categories = await _categoryRepo.GetAllAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList();

                // Ghi log lỗi
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine("ModelState Error: " + error.ErrorMessage);
                    }
                }

                return View(model);
            }

            string imagePath = string.Empty;

            // Xử lý ảnh (chỉ lưu vào wwwroot/images)
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                try
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "images");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    string filePath = Path.Combine(uploadDir, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    imagePath = $"/images/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi lưu ảnh: " + ex.Message);
                    ModelState.AddModelError("", "Không thể lưu hình ảnh. Vui lòng thử lại.");

                    // Load lại danh sách Category khi lỗi
                    var categories = await _categoryRepo.GetAllAsync();
                    model.Categories = categories.Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    }).ToList();

                    return View(model);
                }
            }

            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                Price = model.Price,
                Quantity = model.Quantity,
                Description = model.Description,
                CategoryId = model.CategoryId,
                ImageUrl = imagePath
            };

            try
            {
                await _bookRepository.AddBookAsync(book);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu DB: " + ex.Message);
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu sách vào cơ sở dữ liệu.");

                // Load lại Category khi lỗi DB
                var categories = await _categoryRepo.GetAllAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }).ToList();

                return View(model);
            }

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> EditBook(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewBag.GenreId = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(int id, Book book, IFormFile ImageFile)
        {
            if (id != book.BookId) return NotFound();

            var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.BookId == id);
            if (existingBook == null) return NotFound();

            // Nếu có ảnh mới thì lưu lại ảnh mới, nếu không giữ ảnh cũ
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                book.ImageUrl = "/images/" + fileName;
            }
            else
            {
                book.ImageUrl = existingBook.ImageUrl;
            }

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.BookId == book.BookId))
                    return NotFound();
                else
                    throw;
            }

            ViewBag.GenreId = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }
        
        // GET: Xóa sách
        [HttpGet]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Xác nhận xóa sách
        [HttpPost]
        public async Task<IActionResult> DeleteBookConfirmed(int BookId)
        {
            await _bookRepository.DeleteBookAsync(BookId);
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
