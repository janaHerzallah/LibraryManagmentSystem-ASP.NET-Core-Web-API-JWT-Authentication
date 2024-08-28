using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagmentSystem.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<GetAllBooksResponse>> GetAllBooks();
        Task<GetBookByIdResponse> GetBookById(int id);
        Task<AddBookResponse> AddBookByAdmin(AddBookRequest book);// admin only
        Task<Book> UpdateBookByAdmin(int id, Book updatedBook); // admin only
        Task<bool> DeleteBookByAdmin(int id); // admin only
        Task SoftDeleteBookByAdmin(int id); // admin only
    }
}
