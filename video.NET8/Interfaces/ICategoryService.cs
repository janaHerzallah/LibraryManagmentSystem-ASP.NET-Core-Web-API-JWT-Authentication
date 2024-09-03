using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<GetCategoryResponse>> GetAllCategoriesAsync();
        Task<GetCategoryResponse> GetCategoryByIdAsync(int id);
        Task<AddCategoryResponse> AddCategoryAsync(AddCategoryRequest category);
        Task<UpdateCategoryResponse> UpdateCategoryAsync(int id, UpdateCategoryRequest category);
        Task<bool> DeleteCategoryAsync(int id);
        Task SoftDeleteCategoryAsync(int id);
        Task RemoveBookFromCategoryAsync(int categoryId, int bookId);

        Task<IEnumerable<GetAuthorsBookResponse>> GetBooksInCategoryAsync(int categoryId);

        Task AssignBookToCategoryAsync(int categoryId, int bookId);
    }
}
