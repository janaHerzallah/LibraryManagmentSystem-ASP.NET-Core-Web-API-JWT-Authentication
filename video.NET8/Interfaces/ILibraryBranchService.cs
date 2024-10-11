using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;

namespace LibraryManagmentSystem.Interfaces
{
    public interface ILibraryBranchService
    {
        Task<IEnumerable<GetLibraryBranchResponse>> GetAllBranches();
        Task<IEnumerable<GetLibraryBranchResponse>> GetActiveBranches();
        Task<GetLibraryBranchResponse> GetBranchById(int id);
        Task<AddLibraryBranchResponse> AddBranch(AddLibraryBranchRequest branch);
        Task<UpdateLibraryBranchResponse> UpdateBranch(int id, UpdateLibraryBranchRequest branch);
        Task<bool> DeleteBranch(int id);
        Task SoftDeleteBranch(int id);
        Task<IEnumerable<GetBooksDetailsResponse>> GetBooksInBranch(int branchId);

        Task AssignBookToBranch(int branchId, int bookId);

        Task<bool> RemoveBookFromBranch(int bookId, int branchId);

        Task<IEnumerable<ExcelExportBranchResponse>> ExportBranchesToExcel();
        Task<(List<AddLibraryBranchRequest> validBranches, List<ValidationErrorBranchResponse> validationErrors)> ImportBranchesFromExcel(IFormFile excelFile);




    }
}
