using NhaSachTriThuc.Models;

namespace NhaSachTriThuc.Repositories
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAllBooks();
        IEnumerable<Book> GetBooksByCategory(int categoryId);
        Book GetBookById(int id);
    }
}
