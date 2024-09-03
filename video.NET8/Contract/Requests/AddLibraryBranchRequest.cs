using LibraryManagementSystem.Domain;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddLibraryBranchRequest
    {
        public string? Name { get; set; }

        public string? Location { get; set; }

        public List<AddLibraryBranchBooks> Books { get; set; }

        public AddLibraryBranchRequest()
        {
            Books = new List<AddLibraryBranchBooks>();
        }

    }
}
