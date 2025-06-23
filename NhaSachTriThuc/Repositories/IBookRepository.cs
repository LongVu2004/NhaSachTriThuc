using NhaSachTriThuc.Models;

namespace NhaSachTriThuc.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooks();

        Task<IEnumerable<Book>> GetBooksByCategory(int categoryId);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task <Book> GetBookById(int id);
    }
}
