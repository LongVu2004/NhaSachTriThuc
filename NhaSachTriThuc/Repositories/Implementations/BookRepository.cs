using Microsoft.EntityFrameworkCore;
using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;

namespace NhaSachTriThuc.Repositories.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _context.Books.Include(b => b.Category).ToList();
        }

        public IEnumerable<Book> GetBooksByCategory(int categoryId)
        {
            return _context.Books
                .Include(b => b.Category)
                .Where(b => b.CategoryId == categoryId)
                .ToList();
        }

        public Book GetBookById(int id)
        {
            return _context.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BookId == id);
        }
    }
}
