namespace LibraryManagmentSystem.Contract.Requests
{
    public class UpdateLibraryBranchRequest
    {
        public string? Name { get; set; }

        public string? Location { get; set; }

        public bool Active { get; set; } = true;
    }
}
