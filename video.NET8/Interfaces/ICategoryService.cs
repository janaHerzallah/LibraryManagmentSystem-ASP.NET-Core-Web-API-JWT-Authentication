using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<GetCategoryResponse>> GetAllCategories();
        Task<IEnumerable<GetCategoryResponse>> GetActiveCategories();
        Task<GetCategoryResponse> GetCategoryById(int id);
        Task<AddCategoryResponse> AddCategory(AddCategoryRequest category);
        Task<UpdateCategoryResponse> UpdateCategory(int id, UpdateCategoryRequest category);
        Task<bool> DeleteCategoryAsync(int id);
        Task SoftDeleteCategory(int id);
        Task RemoveBookFromCategoryAsync(int categoryId, int bookId);

        Task<IEnumerable<GetBooksDetailsResponse>> GetBooksInCategory(int categoryId);


        Task AssignBookToCategory(int categoryId, int bookId);

        Task<IEnumerable<GetBooksDetailsResponse>> FilterBooks(int? authorId = null, bool? available = null);


        Task<IEnumerable<GetBooksDetailsResponse>> SearchBooks(string? title = null, string? authorName = null);

        Task<IEnumerable<ExcelExportCategoryResponse>> ExportCategoriesToExcel();
        Task<(List<AddCategoryRequest> validCategories, List<validationErrorCategoryListResponse> validationErrors)> ImportCategoriesFromExcel(IFormFile excelFile);


    }
}
