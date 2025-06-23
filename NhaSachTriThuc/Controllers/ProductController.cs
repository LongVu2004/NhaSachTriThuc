using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;
using NhaSachTriThuc.Repositories;

namespace NhaSachTriThuc.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBookRepository _bookRepository;
        private readonly ICartRepository _cartRepository;

        public ProductController(AppDbContext context, IBookRepository bookRepository,
                                ICartRepository cartRepository)
        {
            _context = context;
            _bookRepository = bookRepository;
            _cartRepository = cartRepository;
        }

        public async Task<IActionResult> Category(int categoryId)
        {
            var books = await _bookRepository.GetBooksByCategory(categoryId);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            ViewData["Name"] = category?.Name ?? "Không xác định";
            return View(books.ToList());
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("Search", new List<Book>()); // Hoặc trả về tất cả sách nếu bạn muốn
            }

            var result = _context.Books
                .Where(b =>
                    b.Title.Contains(query) ||
                    b.Description.Contains(query) ||
                    b.Author.Contains(query))
                .ToList();

            return View("Search", result); // Dùng lại view Index để hiển thị kết quả
        }

        [HttpGet]
        public JsonResult GetSuggestions(string term)
        {
            var suggestions = _context.Books
                .Where(b => b.Title.Contains(term))
                .Select(b => b.Title)
                .Distinct()
                .Take(10)
                .ToList();

            return Json(suggestions);
        }

        public async Task<IActionResult> _DetailsPartial(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null) return NotFound();
            return PartialView("_DetailsPartial", book);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetBookById(id);
            if (book == null) return NotFound();
            return View(book);
        }


    }
}
