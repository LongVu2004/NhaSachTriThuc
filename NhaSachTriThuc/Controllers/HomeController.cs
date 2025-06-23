using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NhaSachTriThuc.Models;
using NhaSachTriThuc.Data;
using Microsoft.EntityFrameworkCore;

namespace NhaSachTriThuc.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/slideshow");
            var imageFiles = Directory.GetFiles(imageFolder)
                .Select(Path.GetFileName) // chỉ lấy tên file
                .ToList();

            ViewBag.ImageList = imageFiles;

            var books = await _context.Books
                .Include(b => b.Category)
                .OrderByDescending(b => b.BookId)
                .ToListAsync();

            return View(books);
        }

        public IActionResult Category(int categoryId)
        {
            var books = _context.Books
                .Where(b => b.CategoryId == categoryId)
                .Include(b => b.Category)
                .ToList();

            return View(books); // tạo Views/Product/Category.cshtml để hiển thị
        }

        public IActionResult DetailsPartial(int id)
        {
            var book = _context.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BookId == id);

            if (book == null) return NotFound();

            return PartialView("_BookDetailsPartial", book);
        }
        [HttpGet]
        public IActionResult FilterByPrice(int? range)
        {
            var books = _context.Books.AsQueryable();

            switch (range)
            {
                case 1: // Dưới 50k
                    books = books.Where(b => b.Price < 50000);
                    break;
                case 2: // 50k - 100k
                    books = books.Where(b => b.Price >= 50000 && b.Price <= 100000);
                    break;
                case 3: // Trên 100k
                    books = books.Where(b => b.Price > 100000);
                    break;
            }

            return View("Index", books.ToList());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
