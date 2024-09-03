namespace LibraryManagmentSystem.Contract.Responses
{
    public class UpdateLibraryBranchResponse
    {

        public string? Name { get; set; }

        public string? Location { get; set; }

        public bool Active { get; set; } = true;
        public int BranchId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

    }
}
