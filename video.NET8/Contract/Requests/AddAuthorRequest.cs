using LibraryManagementSystem.Contract.Requests;
using LibraryManagementSystem.Domain;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagmentSystem.Contract.Requests
{
    public class AddAuthorRequest
    {
        // i will try adding an aithor with its books directly
        // and i want to try if the list is reomved from the parameters would it just nt make a book for the author ?
        // if it causes a problem , then i will make a separte request for adding a book


        public string? Name { get; set; }

        public List<AddAuthorsBooksRequest> Books { get; set; }


        public AddAuthorRequest()
        {
            Books = new List<AddAuthorsBooksRequest>();
        }
    }
}
