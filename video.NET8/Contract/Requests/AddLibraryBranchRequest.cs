using LibraryManagementSystem.Domain;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddLibraryBranchRequest
    {

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string? Location { get; set; }

        public List<AddLibraryBranchBooks> Books { get; set; }

        public AddLibraryBranchRequest()
        {
            Books = new List<AddLibraryBranchBooks>();
        }

    }
}
