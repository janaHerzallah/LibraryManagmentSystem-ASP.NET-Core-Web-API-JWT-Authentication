using LibraryManagmentSystem.Contract.Requests;
using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Responses;
namespace LibraryManagmentSystem.Interfaces
{
    public interface IUserService
    {
        Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request);// update to return type of RegisterUserResponse

        Task<LogInUserResponse>LogInUser(LogInUserRequest request);

        Task<bool> ValidateAdminsToken(string token);
        Task<bool> ValidateUsersToken(string token);
        Task<IEnumerable<GetUserResponse>> GetMembersByAdminOnly();

        Task<IEnumerable<GetUserResponse>> GetActiveAndInActiveMembersByAdminOnly();
        Task<ActivateAndDeactivateUserResponse> DeactivateUser(ActivateDeActivateUserRequest request);

        Task<ActivateAndDeactivateUserResponse> ReActivateUser(ActivateDeActivateUserRequest request);
    }
}
