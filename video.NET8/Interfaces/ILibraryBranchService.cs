using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;

namespace LibraryManagmentSystem.Interfaces
{
    public interface ILibraryBranchService
    {
        Task<IEnumerable<GetLibraryBranchResponse>> GetAllBranchesAsync();
        Task<GetLibraryBranchResponse> GetBranchByIdAsync(int id);
        Task<AddLibraryBranchResponse> AddBranchAsync(AddLibraryBranchRequest branch);
        Task<UpdateLibraryBranchResponse> UpdateBranchAsync(int id, UpdateLibraryBranchRequest branch);
        Task<bool> DeleteBranchAsync(int id);
        Task SoftDeleteBranchAsync(int id);
        Task<IEnumerable<GetAuthorsBookResponse>> GetBooksInBranchAsync(int branchId);

        Task AssignBookToBranchAsync(int branchId, int bookId);

        Task<bool> RemoveBookFromBranchAsync(int bookId, int branchId);



    }
}
