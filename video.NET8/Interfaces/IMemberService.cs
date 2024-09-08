using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;
using LibraryManagmentSystem.Contract.Responses;


namespace LibraryManagementSystem.Interfaces
    {
        public interface IMemberService
        {

        Task<IEnumerable<GetMemberResponse>> GetAllMembers();
            Task<IEnumerable<GetMemberResponse>> GetActiveMembers();
            Task<GetMemberResponse> GetMemberById(int id);
            Task<GetMemberResponse> AddMember(AddMemberRequest member);
            Task<GetMemberResponse> UpdateMember(int id, UpdateMemeberRequest updatedMember);
            Task<bool> DeleteMember(int id);
            Task SoftDeleteMember(int id);

            Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetMembersAllBorrowedBooks(int memberId,string token);

            Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetNotReturnedBooks(int memberId, string token);
            Task<IEnumerable<GetBorrowedBooksForAMemberResponse>> GetOverDueBorrowedBooks(int memberId, string token);

            Task<int> GetOverdueBooksCount(int memberId, string token);
        }
    }




