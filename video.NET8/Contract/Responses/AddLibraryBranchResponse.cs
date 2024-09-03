using LibraryManagementSystem.Domain;
using LibraryManagmentSystem.Contract.Requests;

namespace LibraryManagmentSystem.Contract.Responses
{
    public class AddLibraryBranchResponse: DateBaseClass
    {

        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Location { get; set; }

        public List<AddLibraryBranchBooks> Books { get; set; }

        public AddLibraryBranchResponse()
        {
            Books = new List<AddLibraryBranchBooks>();
        }

    }
}
