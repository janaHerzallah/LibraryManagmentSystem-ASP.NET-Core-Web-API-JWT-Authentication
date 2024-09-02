using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Responses;

namespace LibraryManagementSystem.Interfaces
{
    public interface IBorrowService
    {
        Task<BorrowBookResponse> BorrowBook(int memberId, int bookId);
        Task<ReturnBookResponse> ReturnBook(int memberId, int bookId);
        Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetBorrowedBooksByMember(int memberId);


    }
}
