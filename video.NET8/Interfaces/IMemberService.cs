using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;


namespace LibraryManagementSystem.Interfaces
    {
        public interface IMemberService
        {
            Task<IEnumerable<GetMemberResponse>> GetAllMembersAsync();
            Task<GetMemberResponse> GetMemberByIdAsync(int id);
            Task<GetMemberResponse> AddMemberAsync(AddMemberRequest member);
            Task<GetMemberResponse> UpdateMemberAsync(int id, UpdateMemeberRequest updatedMember);
            Task<bool> DeleteMemberAsync(int id);
            Task SoftDeleteMemberAsync(int id);

            // three get APIS related to the borrow table still needs to be figured out

            Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetBorrowedBooksByMemberAsync(int memberId);

            Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetBorrowedBooksNotReturnedByMemberAsync(int memberId);

            Task<int> GetOverdueBooksCountByMemberAsync(int memberId);
        }
    }




