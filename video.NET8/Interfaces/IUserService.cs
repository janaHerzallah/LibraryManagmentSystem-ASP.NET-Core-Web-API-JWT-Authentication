using LibraryManagmentSystem.Contract.Requests;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Responses;
namespace LibraryManagmentSystem.Interfaces
{
    public interface IUserService
    {
        Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request);// update to return type of RegisterUserResponse

        Task<LogInUserResponse>LogInUser(LogInUserRequest request);

        Task<bool> ValidateToken(string token);

        Task<IEnumerable<User>> GetMembersByAdminOnly();
    }
}
