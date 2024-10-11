using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagmentSystem.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<GetAllAuthorsResponse>> GetAllAuthors(); //admin only
        Task<IEnumerable<GetAllAuthorsResponse>> GetActiveAuthors();
        Task<GetAuthorByIdResponse> GetAuthorById(int id);
        Task<AddAuthorResponse> AddAuthor(AddAuthorRequest author); // admin only
        Task<UpdateAuthorResponse> UpdateAuthor(int id, UpdateAuthorRequest updatedAuthor); // admin only
        Task<bool> DeleteAuthor(int id); // admin only
        Task SoftDeleteAuthor(int id); // admin only

       Task<IEnumerable<ExcelExportAuthorResponse>> ExportAllAuthorsToExcel();

        Task<IEnumerable<ExcelExportAuthorResponse>> ExportActiveAuthorsToExcel();

        Task<(List<AddAuthorRequest> validAuthors, List<validationErrorAuthorListResponse> validationErrors)> ImportAuthorsFromExcel(IFormFile excelFile);
    }
}
