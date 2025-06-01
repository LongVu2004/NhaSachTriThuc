using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaSachTriThuc.Data;
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

        public IActionResult Category(int categoryId)
        {
            var books = _bookRepository.GetBooksByCategory(categoryId);
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            ViewData["Name"] = category?.Name ?? "Không xác định";
            return View(books.ToList());
        }

        public IActionResult _DetailsPartial(int id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null) return NotFound();
            return PartialView("_DetailsPartial", book);
        }
        
        public IActionResult Details(int id)
        {
            var book = _bookRepository.GetBookById(id);
            if (book == null)
                return NotFound();

            return View(book); // Trả về Views/Product/Details.cshtml
        }
    }
}
