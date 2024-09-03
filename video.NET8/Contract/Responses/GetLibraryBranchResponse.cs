using LibraryManagmentSystem.Contract.Responses;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class GetLibraryBranchResponse
    {

        public int BranchId { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }

        public bool Active { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public List<GetAuthorsBookResponse> Books { get; set; }

    }
}
