using LibraryManagementSystem.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagmentSystem.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(int id, Book updatedBook);
        Task<bool> DeleteBookAsync(int id);
    }
}
