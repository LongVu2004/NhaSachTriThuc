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
        public async Task <IEnumerable<Book>> GetAllBooks()
        {
            //return _context.Books.Include(b => b.Category).ToList();
            return await _context.Books.Include(b => b.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByCategory(int categoryId)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Where(b => b.CategoryId == categoryId)
                .ToListAsync();
        }
        public async Task AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task<Book> GetBookById(int id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteBookAsync(int id)
        {
            var book = _context.Books.FindAsync(id);
            _context.Books.Remove(book.Result);
            await _context.SaveChangesAsync();
        }
    }
}
