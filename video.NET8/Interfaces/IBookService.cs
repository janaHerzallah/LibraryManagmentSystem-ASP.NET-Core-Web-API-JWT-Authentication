using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagmentSystem.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<GetAllBooksResponse>> GetAllBooks(); //admin only
        Task<IEnumerable<GetAllBooksResponse>> GetActiveBooks();
        Task<GetBookByIdResponse> GetBookById(int id);
        Task<AddBookResponse> AddBook(AddBookRequest book);// admin only
        Task<AddBookResponse> UpdateBook(int id, UpdateBookRequest updatedBook); // admin only
        Task<bool> DeleteBook(int id); // admin only
        Task SoftDeleteBook(int id); // admin only

        Task<(List<AddBookRequest> validBooks, List<validationErrorBookListResponse> validationErrors)> ImportBooksFromExcel(IFormFile excelFile);
    }
}
