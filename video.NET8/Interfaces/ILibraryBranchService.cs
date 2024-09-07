using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;

namespace LibraryManagmentSystem.Interfaces
{
    public interface ILibraryBranchService
    {
        Task<IEnumerable<GetLibraryBranchResponse>> GetActiveAndInActiveBranches();
        Task<IEnumerable<GetLibraryBranchResponse>> GetActiveBranches();
        Task<GetLibraryBranchResponse> GetBranchById(int id);
        Task<AddLibraryBranchResponse> AddBranchByAdmin(AddLibraryBranchRequest branch);
        Task<UpdateLibraryBranchResponse> UpdateBranchByAdmin(int id, UpdateLibraryBranchRequest branch);
        Task<bool> DeleteBranchByAdmin(int id);
        Task SoftDeleteBranchByAdmin(int id);
        Task<IEnumerable<GetBooksDetailsResponse>> GetBooksInBranch(int branchId);

        Task AssignBookToBranchByAdmin(int branchId, int bookId);

        Task<bool> RemoveBookFromBranchByAdmin(int bookId, int branchId);



    }
}
