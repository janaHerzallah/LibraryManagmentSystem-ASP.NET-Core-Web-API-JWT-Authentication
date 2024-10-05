using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;

namespace LibraryManagementSystem.Interfaces
{
    public interface IBorrowService
    {
        Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request , string token);
        Task<ReturnBookResponse> ReturnBook(int memberId, int bookId , string token);
        Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetMembersBorrowedBooks(int memberId, string token);

        Task<(List<Borrow> validBorrows, List<ValidationErrorBorrowResponse> validationErrors)> ImportBorrowsFromExcel(IFormFile excelFile);

        Task AddBorrowRecordfromExcel(Borrow borrow);

        Task<IEnumerable<ExcelBorrowBookResponse>> ExportAllBorrowsTOExcel();
    }
}
