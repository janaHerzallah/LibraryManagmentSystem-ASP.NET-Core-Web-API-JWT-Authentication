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
        Task<IEnumerable<GetAllAuthorsResponse>> GetAllAuthors();
        Task<GetAuthorByIdResponse> GetAuthorById(int id);
        Task<AddAuthorResponse> AddAuthorByAdmin(AddAuthorRequest author); // admin only
        Task<UpdateAuthorResponse> UpdateAuthorByAdmin(int id, UpdateAuthorRequest updatedAuthor); // admin only
        Task<bool> DeleteAuthorByAdmin(int id); // admin only
        Task SoftDeleteAuthorByAdmin(int id); // admin only
    }
}
